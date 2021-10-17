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
        private RoadEntryInstance moveobj;
        private RoadEntryInstance lastobj;
        private GameObject parentobj;
        private Text modeobj;
        private RoadPartsSelector parts_selector;

        void Start()
        {
            parentobj = GameObject.Find("Roads");
            modeobj = GameObject.Find("RoadEditorModeInfo").GetComponent<Text>();
            parts_selector = parentobj.GetComponent<RoadPartsSelector>();
            Debug.Log("modeobj=" + modeobj.text);
            this.moveobj = null;
            this.lastobj = null;
        }


        public void CreateParts()
        {
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
                entry.prefab_name = this.parts_selector.GetCurrentSelectedPartsName();
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
        private void DoMoveArround(RoadEntryInstance obj, RoadEntryInstance base_obj)
        {
            bool is_move = false;
            // 左に移動
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.H))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "-x" +"/" + base_obj.instance.name;
            }
            // 右に移動
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "+x" + "/" + base_obj.instance.name;
            }
            // 前に移動
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.I))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "+z" + "/" + base_obj.instance.name;
            }
            // 後ろに移動
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.N))
            {
                is_move = true;
                obj.cfg_entry.connect_direction = "-z" + "/" + base_obj.instance.name;
            }
            if (is_move)
            {
                try
                {
                    RoadEntryPositionCalculator.CalculatePos(base_obj, obj);
                    obj.instance.transform.position = obj.pos;
                }
                catch (Exception)
                {
                    Debug.Log("ERROR: Can not move this way");
                }
            }
        }

        private RoadEntryInstance GetSelectedInstance()
        {
            return RoadEntryInstance.GetInstanceFromIndex(RoadEntrySelector.GetSelectedIndex());
        }

        private void DoSelect(KeyCode code)
        {
            RoadEntryInstance selected = null;
            RoadEntrySelector.DoSelect(code, this.lastobj);
            selected = GetSelectedInstance();
            if (selected != null)
            {
                this.DoSelect();
            }
        }
        private void DoSelect()
        {
            var selected = GetSelectedInstance();
            if (selected != null)
            {
                this.SelectObject(selected.instance);
                Debug.Log("selected obj=" + selected.instance.name);
            }
        }

        void Update()
        {
            if (this.moveobj != null)
            {
                //******************MOVE MODE****************
                this.modeobj.text = "MODE: MOVE";
                //Debug.Log("lastobj=" + lastobj);
                if (this.lastobj == null)
                {
                    DoMove(this.moveobj.instance);
                }
                else
                {
                    //Debug.Log("move=" + this.moveobj.instance.name + " base=" + this.lastobj.instance.name);
                    DoMoveArround(this.moveobj, this.lastobj);
                }
                DoRotate(this.moveobj);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    RoadEntryInstance.AddInstance(this.moveobj);
                    this.moveobj.instance.transform.parent = this.parentobj.transform;
                    this.moveobj.pos.x = this.moveobj.instance.transform.position.x;
                    this.moveobj.pos.z = this.moveobj.instance.transform.position.z;
                    //Debug.Log("moveobj pos=" + this.moveobj.pos);
                    //this.moveobj.instance.name = this.moveobj.instance.name + "_" + RoadEntryComposer.GetLastObjectIndex();
                    RoadEntrySelector.DoSelectLastObj();
                    this.DoSelect();
                    this.lastobj = this.moveobj;
                    this.moveobj = null;
                }
            }
            else
            {
                this.modeobj.text = "MODE: CREATE";
                //******************CREATE MODE****************
                KeyCode code = KeyCode.None;
                if (Input.GetKeyDown(KeyCode.C))
                {
                    this.CreateParts();
                }
                else if (RoadEntrySelector.is_select_key_down(out code))
                {
                    DoSelect(code);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    //Select object
                    this.parts_selector.DoUpdateParts(true);
                }
                else if (Input.GetKeyDown(KeyCode.K))
                {
                    //Select object
                    this.parts_selector.DoUpdateParts(false);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    //Deselect object
                    RoadEntrySelector.DoDeselect();
                    this.DeSelectObject();
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DestoryObject();
                }
#if false
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (RoadEntrySelector.GetSelectedIndex() >= 0)
                    {
                        this.lastobj = RoadEntryInstance.GetInstanceFromIndex(RoadEntrySelector.GetSelectedIndex());
                    }
                }
#endif
                if (Input.GetKeyDown(KeyCode.M))
                {
                    if (RoadEntrySelector.GetSelectedIndex() >= 0)
                    {
                        this.moveobj = RoadEntryInstance.GetInstanceFromIndex(RoadEntrySelector.GetSelectedIndex());
                    }
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    RoadSaver.Save();
                }
            }
        }
        private GameObject select_obj = null;

        private void DeSelectObject()
        {
            //Selection.objects = new UnityEngine.Object[0];
            if (this.select_obj != null)
            {
                Destroy(this.select_obj);
            }
        }
        private void SelectObject(GameObject obj)
        {
            //var objs = new UnityEngine.Object[1];
            //objs[0] = obj;
            //Selection.objects = objs;
            this.DeSelectObject();
            this.lastobj = RoadEntryInstance.GetInstanceFromIndex(RoadEntrySelector.GetSelectedIndex());
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(2, 1, 2);
            var pos = obj.GetComponent<Transform>();
            cube.transform.position = new Vector3(pos.position.x, pos.position.y+10, pos.position.z);
            cube.transform.Rotate(0, 45, 0);
            this.select_obj = cube;
            cube.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }

        private void DestoryObject()
        {
            if (RoadEntrySelector.GetSelectedIndex() >= 0)
            {
                var e = GetSelectedInstance();
                if (e != null)
                {
                    RoadEntryInstance.RemoveInstance(e);
                    this.lastobj = RoadEntryInstance.GetLastObj();
                    RoadEntryComposer.DestroyOne(e);
                    
                    RoadEntrySelector.DoDeselect();
                    this.DeSelectObject();

                    RoadEntrySelector.DoSelectLastObj();
                    this.DoSelect();
                }
            }
        }
        public void DestroyAll()
        {
            RoadEntrySelector.DoSelectLastObj();
            while (RoadEntrySelector.GetSelectedIndex() >= 0)
            {
                this.DestoryObject();
                RoadEntrySelector.DoSelectLastObj();
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
            //var p = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var p = Resources.Load<GameObject>(path);
            if (p == null)
            {
                throw new InvalidDataException("ERROR: path is not found:" + path);
            }
            return Instantiate(p, this.transform.position, Quaternion.identity) as GameObject;
        }
    }

}
