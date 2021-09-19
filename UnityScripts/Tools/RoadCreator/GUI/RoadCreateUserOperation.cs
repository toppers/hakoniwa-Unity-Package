using Hakoniwa.Tools.RoadMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hakoniwa.Tools.RoadMap
{
    public class RoadCreateUserOperation : MonoBehaviour
    {
        private int obj_index = 0;
        private RoadEntryInstance moveobj;
        private GameObject parentobj;
        private RoadEntryInstance lastobj;
        private Dropdown road_type_obj;
        //private string base_medium_path = "Assets/CityVoxelPack/Assets/roads/medium/Prefabs/";
        private List<RoadEntryInstance> road_objs = new List<RoadEntryInstance>();
        private int select_index = -1;

        public Sprite[] images;
        private string[] prefab_names;
        private Image myImage;

        void Start()
        {
            parentobj = GameObject.Find("Roads");
            road_type_obj = GameObject.Find("RoadTypes").GetComponent<Dropdown>();
            this.moveobj = null;
            this.lastobj = null;
            this.myImage = GameObject.Find("Image").GetComponent<Image>();
            this.prefab_names = new string[17];
            this.prefab_names[0] = "road1";
            this.prefab_names[1] = "road2";
            this.prefab_names[2] = "road3";
            this.prefab_names[3] = "road3bis";
            this.prefab_names[4] = "road4";
            this.prefab_names[5] = "road5";
            this.prefab_names[6] = "road10"; //d_junction
            this.prefab_names[7] = "road14bis"; //d_junction1
            this.prefab_names[8] = "road13"; //d_junction2
            this.prefab_names[9] = "road13bis"; //d_junction3
            this.prefab_names[10] = "road14"; //d_junction4
            this.prefab_names[11] = "road14bisbis"; //d_junction5
            this.prefab_names[12] = "road11"; //d_straight
            this.prefab_names[13] = "road12"; //d_corner
            this.prefab_names[14] = "road12bis"; //d_corner_junction1
            this.prefab_names[15] = "road12bisbis"; //d_corner_junction2
            this.prefab_names[16] = "road15"; //d_cross_road
            Debug.Log("MousleClickEventHandler:" + road_type_obj.value);
        }


        public void CreateParts()
        {
            if (this.select_index < 0)
            {
                if (this.road_objs.Count > 0)
                {
                    this.select_index = this.road_objs.Count - 1;
                    this.lastobj = GetSelectedInstance();
                }
            }

            if ((this.moveobj == null))
            {
                RoadEntryInstance tmpobj = null;
                RoadMapEntry entry = new RoadMapEntry();
                entry.rotation = 0.0f;
                entry.scale = 1.0f;
                if (this.lastobj != null)
                {
                    entry.connect_direction = "+z";
                    tmpobj = this.lastobj;
                }
                entry.prefab_name = this.prefab_names[road_type_obj.value];
                this.moveobj = RoadEntryComposer.CreateEntry(ref tmpobj, entry);
            }
        }

        private void DoMove(GameObject obj)
        {
            Vector3 diffposi;
            diffposi.x = 0;
            diffposi.z = 0;
            diffposi.y = 0;

            float scale = 1.0f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                scale = 0.1f;
            }

            // 左に移動
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.H))
            {
                diffposi.x = -0.1f * scale;
            }
            // 右に移動
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.L))
            {
                diffposi.x = 0.1f * scale;
            }
            // 前に移動
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
            {
                diffposi.z = 0.1f * scale;
            }
            // 後ろに移動
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.N))
            {
                diffposi.z = -0.1f * scale;
            }
            obj.transform.position += diffposi;
        }
        private void DoMoveArround(RoadEntryInstance obj)
        {
            bool is_move = false;
            // 左に移動
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.H))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "-x";
            }
            // 右に移動
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.L))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "+x";
            }
            // 前に移動
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "+z";
            }
            // 後ろに移動
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.N))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "-z";
            }
            if (is_move)
            {
                try
                {
                    RoadEntryPositionCalculator.CalculatePos(this.lastobj, this.moveobj);
                    this.moveobj.instance.transform.position = this.moveobj.pos;
                }
                catch (Exception)
                {
                    Debug.Log("ERROR: Can not move this way");
                }
            }
        }

        private RoadEntryInstance GetSelectedInstance()
        {
            int index = 0;
            foreach (var e in this.road_objs)
            {
                if (index == this.select_index)
                {
                    //Debug.Log("name=" + e.instance.name);
                    return e;
                }
                index++;
            }
            return null;
        }

        private void DoSelect(KeyCode code)
        {
            RoadEntryInstance selected = null;
            int index = 0;

            if (this.select_index < 0)
            {
                index = 0;
                foreach (var e in this.road_objs)
                {
                    Debug.Log("select kouho:" + e.instance.name);
                    if (e == this.lastobj)
                    {
                        this.select_index = index;
                        Debug.Log("select obj:" + e.instance.name);
                        break;
                    }
                    index++;
                }
                if (this.select_index < 0)
                {
                    this.select_index = this.road_objs.Count - 1;
                }
            }

            if ((code == KeyCode.UpArrow) || (code == KeyCode.RightArrow) || (code == KeyCode.H))
            {
                if (this.select_index < (this.road_objs.Count - 1))
                {
                    this.select_index = this.select_index + 1;
                }
                else
                {
                    this.select_index = 0;
                }
            }
            else if ((code == KeyCode.DownArrow) || (code == KeyCode.LeftArrow) || (code == KeyCode.L))
            {
                if (this.select_index == 0)
                {
                    this.select_index = (this.road_objs.Count - 1);
                }
                else
                {
                    this.select_index = this.select_index - 1;
                }
            }

            selected = GetSelectedInstance();
            if (selected != null)
            {
                var objs = new UnityEngine.Object[1];
                objs[0] = selected.instance;
                Selection.objects = objs;
                this.lastobj = selected;
            }
        }
        private void DoSelect(int index)
        {
            this.select_index = index;
            var selected = GetSelectedInstance();
            if (selected != null)
            {
                var objs = new UnityEngine.Object[1];
                objs[0] = selected.instance;
                Selection.objects = objs;
                this.lastobj = selected;
            }
        }
        private void DoDeselect()
        {
            //Debug.Log("Deselect enter");
            Selection.objects = new UnityEngine.Object[0];
            this.select_index = -1;
        }

        private bool is_select_key_down(out KeyCode code)
        {
            bool is_select = true;
            if (Input.GetKeyDown(KeyCode.S))
            {
                code = KeyCode.S;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                code = KeyCode.LeftArrow;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                code = KeyCode.RightArrow;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                code = KeyCode.DownArrow;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                code = KeyCode.UpArrow;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                code = KeyCode.H;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                code = KeyCode.LeftArrow;
            }
            else
            {
                is_select = false;
                code = KeyCode.None;
            }
            return is_select;
        }

        void Update()
        {
            this.myImage.sprite = images[road_type_obj.value];
            if (this.moveobj != null)
            {
                //Debug.Log("lastobj=" + lastobj);
                if (this.lastobj == null)
                {
                    DoMove(this.moveobj.instance);
                }
                else
                {
                    DoMoveArround(this.moveobj);
                }
                DoRotate(this.moveobj);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    this.road_objs.Add(this.moveobj);
                    this.moveobj.pos.x = this.moveobj.instance.transform.position.x;
                    this.moveobj.pos.z = this.moveobj.instance.transform.position.z;
                    //Debug.Log("moveobj pos=" + this.moveobj.pos);
                    this.moveobj.instance.name = this.moveobj.instance.name + "_" + this.obj_index;
                    this.DoSelect(this.obj_index);
                    this.obj_index++;
                    this.moveobj.instance.transform.parent = this.parentobj.transform;
                    if (this.lastobj == null)
                    {
                        this.lastobj = this.moveobj;
                    }
                    this.moveobj = null;
                }
            }
            else
            {
                KeyCode code = KeyCode.None;
                if (Input.GetKeyDown(KeyCode.C))
                {
                    this.CreateParts();
                }
                else if (is_select_key_down(out code))
                {
                    DoSelect(code);
                }
                else if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K))
                {
                    //Select object
                    DoUpdateParts(true);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    //Deselect object
                    DoDeselect();
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DestoryObject();
                }
            }
        }
        private int parts_index = 0;
        private void DoUpdateParts(bool way)
        {
            if (way)
            {
                parts_index++;
                if (parts_index >= prefab_names.Length)
                {
                    parts_index = 0;
                }

            }
            else
            {
                if (parts_index == 0)
                {
                    parts_index = prefab_names.Length - 1;
                }
                else
                {
                    parts_index--;
                }
            }
            road_type_obj.value = parts_index;
        }

        private void DestoryObject()
        {
            if (this.select_index >= 0)
            {
                var e = GetSelectedInstance();
                this.road_objs.Remove(e);
                if (this.lastobj == e)
                {
                    this.lastobj = null;
                }
                RoadEntryComposer.DestroyOne(e);
                Selection.objects = new UnityEngine.Object[0];
                this.select_index = -1;
            }
        }

        private void DoRotate(RoadEntryInstance obj)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                obj.instance.transform.Rotate(0, 90, 0);
                obj.cfg_entry.rotation += 90.0f;
                if (obj.cfg_entry.rotation > 360.0f)
                {
                    obj.cfg_entry.rotation = 0.0f;
                }
            }
        }

        private GameObject CreateParts(string path)
        {
            var p = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (p == null)
            {
                throw new InvalidDataException("ERROR: path is not found:" + path);
            }
            return Instantiate(p, this.transform.position, Quaternion.identity) as GameObject;
        }
    }

}
