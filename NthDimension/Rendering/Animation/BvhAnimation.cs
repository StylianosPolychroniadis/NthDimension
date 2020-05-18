/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NthDimension.Rendering.Animation
{
    public class BvhAnimation
    {
        enum BvhMode
        {
            UNKNOWN,
            HIERARCHY,
            MOTION,
        };

        #region class BvhVec3
        public struct BvhVec3
        {
            public float X { set; get; }
            public float Y { set; get; }
            public float Z { set; get; }

            public BvhVec3(float x, float y, float z)
                : this()
            {
                X = x; Y = y; Z = z;
            }
        };
        #endregion

        #region class BvhNode
        public class BvhNode
        {
            public enum Channel
            {
                UNKNOWN,
                Xposition, Yposition, Zposition,
                Xrotation, Yrotation, Zrotation,
            };

            public enum eElement
            {
                X,
                Y,
                Z,
            };

            public BvhNode                      Parent { set; get; }
            public String                       Name { set; get; }
            public BvhVec3                      Offset { set; get; }
            public List<Channel>                Channnels { set; get; }
            public List<BvhNode>                Nodes { set; get; }

            private Dictionary<int, BvhVec3>    motionPos;
            private Dictionary<int, BvhVec3>    motionRot;
            public void SetMotionPos(int frame, BvhVec3 pos)
            {
                if (motionPos.ContainsKey(frame) == false) motionPos.Add(frame, new BvhVec3());
                motionPos[frame] = pos;
            }
            public void SetMotionPos(int frame, float pos, eElement e)
            {
                if (motionPos.ContainsKey(frame) == false) motionPos.Add(frame, new BvhVec3());
                BvhVec3 newVal = motionPos[frame];
                switch (e)
                {
                    case eElement.X: newVal.X = pos; break;
                    case eElement.Y: newVal.Y = pos; break;
                    case eElement.Z: newVal.Z = pos; break;
                }
                motionPos[frame] = newVal;
            }
            public void SetMotionRot(int frame, BvhVec3 rot)
            {
                if (motionRot.ContainsKey(frame) == false) motionRot.Add(frame, new BvhVec3());
                motionRot[frame] = rot;
            }
            public void SetMotionRot(int frame, float rot, eElement e)
            {
                if (motionRot.ContainsKey(frame) == false) motionRot.Add(frame, new BvhVec3());
                BvhVec3 newVal = motionRot[frame];
                switch (e)
                {
                    case eElement.X: newVal.X = rot; break;
                    case eElement.Y: newVal.Y = rot; break;
                    case eElement.Z: newVal.Z = rot; break;
                }
                motionRot[frame] = newVal;
            }
            public BvhVec3 GetMotionPos(int frame)
            {
                return motionPos.ContainsKey(frame) ? motionPos[frame] : new BvhVec3(0, 0, 0);
            }
            public BvhVec3 GetMotionRot(int frame)
            {
                return motionRot.ContainsKey(frame) ? motionRot[frame] : new BvhVec3(0, 0, 0);
            }

            public BvhNode()
            {
                Channnels = new List<Channel>();
                Nodes = new List<BvhNode>();
                motionPos = new Dictionary<int, BvhVec3>();
                motionRot = new Dictionary<int, BvhVec3>();
            }
        };
        #endregion

        #region Properties
        public bool                             IsEnable { get; private set; }
        public int                              FrameNum { get; private set; }
        public float                            FrameSpan { get; private set; }
        #endregion

        private BvhMode                         _mode;

        private BvhNode                         _root;
        private BvhNode _target;

        private List<BvhNode>                   _nodeOrder;                                 // motion
        private Dictionary<string, BvhNode>     _nodeDic;	                                // pull node instance from node name
        private bool                            isFrameNum      = false;
        private bool                            isFrameSpan     = false;
        private int                             motionNum       = 0;


        #region Ctor
        public BvhAnimation()
        {
            _mode = BvhMode.UNKNOWN;
            _nodeOrder = new List<BvhNode>();
            _nodeDic = new Dictionary<string, BvhNode>();
            IsEnable = false;
        }
        #endregion

        #region Load Bvh data
        public void Load(string filepath)
        {
            _mode = BvhMode.UNKNOWN;
            _root = null;
            _target = null;
            isFrameNum = false;
            isFrameSpan = false;
            motionNum = 0;
            _nodeOrder.Clear();
            _nodeDic.Clear();

            using (StreamReader sr = new StreamReader(filepath, System.Text.Encoding.GetEncoding("shift_jis")))
                Load(sr);
        }
        public void Load(StreamReader sr)
        {
            while (sr.Peek() != -1)
            {
                // split
                String[] words = sr.ReadLine().Split(' ');

                for (int i = 0; i < words.Length; i++)
                    words[i] = words[i].Trim();
                
                List<string> word_list = new List<string>();
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i] != "")
                        word_list.Add(words[i]);
                }

                if (word_list.Count == 0) continue;

                if (word_list[0] == "HIERARCHY")
                {
                    _mode = BvhMode.HIERARCHY;
                    continue;
                }
                else if (word_list[0] == "MOTION")
                {
                    _mode = BvhMode.MOTION;
                    continue;
                }
                switch (_mode)
                {
                    case BvhMode.HIERARCHY:
                        if (word_list[0] == "End" && word_list[1] == "Site")
                        {
                            sr.ReadLine();
                            sr.ReadLine();
                            sr.ReadLine();
                            break;
                        }

                        if (parseHierarchy(word_list.ToArray()) == false)
                        {
                            //System.Windows.Forms.MessageBox.Show("ParseHierarchy Error");
                            return;
                        }
                        break;

                    case BvhMode.MOTION:
                        if (parseMotion(word_list.ToArray()) == false)
                        {
                            //System.Windows.Forms.MessageBox.Show("ParseMotion Error");
                            return;
                        }
                        break;

                    default: break;
                }
            } //end while

            foreach (var node in _nodeOrder)
                _nodeDic.Add(node.Name, node);

            IsEnable = true;
        }

        private bool parseHierarchy(String[] words)
        {
            switch (words[0])
            {
                //Root
                case "ROOT":
                    {
                        _root = new BvhNode();
                        _root.Parent = null;
                        _root.Name = words[1];
                        _target = _root;

                        _nodeOrder.Add(_target);
                    }
                    break;

                case "{":
                    {

                    }
                    break;

                //Return parent-child hierarchy by one level
                case "}":
                    {
                        _target = _target.Parent;
                    }
                    break;
                case "OFFSET":
                    {
                        _target.Offset = new BvhVec3(float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]));
                    }
                    break;

                // Cooperation part with information motion part included in node
                case "CHANNELS":
                    {
                        int chNum = int.Parse(words[1]);    // CHANNNEL NUMBER
                        for (int i = 0; i < chNum; i++)
                        {
                            BvhNode.Channel channel = BvhNode.Channel.UNKNOWN;
                            if (words[i + 2] == BvhNode.Channel.Xposition.ToString()) channel = BvhNode.Channel.Xposition;
                            else if (words[i + 2] == BvhNode.Channel.Yposition.ToString()) channel = BvhNode.Channel.Yposition;
                            else if (words[i + 2] == BvhNode.Channel.Zposition.ToString()) channel = BvhNode.Channel.Zposition;
                            else if (words[i + 2] == BvhNode.Channel.Xrotation.ToString()) channel = BvhNode.Channel.Xrotation;
                            else if (words[i + 2] == BvhNode.Channel.Yrotation.ToString()) channel = BvhNode.Channel.Yrotation;
                            else if (words[i + 2] == BvhNode.Channel.Zrotation.ToString()) channel = BvhNode.Channel.Zrotation;

                            if (channel == BvhNode.Channel.UNKNOWN) return false;   
                            _target.Channnels.Add(channel);
                        }
                    }
                    break;

                // Add a child joint and set it as a target node
                case "JOINT":
                    {
                        var child       = new BvhNode();
                        child.Parent    = _target;          // Parent is the current target node
                        child.Name      = words[1];         // Joint name
                        _target.Nodes.Add(child);           // Add child joint
                        _target         = child;            // Reset target node
                        _nodeOrder.Add(_target);            // store order
                    }
                    break;

                default: break;
            }
            return true;
        }
        private bool parseMotion(String[] words)
        {
            if (!isFrameNum || !isFrameSpan)
            {
                //Frame数
                if (words[0] == "Frames:")
                {
                    FrameNum = int.Parse(words[1]);
                    isFrameNum = true;
                }
                else if (words[0] == "Frames" && words[1] == ":")
                {
                    FrameNum = int.Parse(words[2]);
                    isFrameNum = true;
                }
                //Frame間隔
                else if (words[0] == "Frame" && words[1] == "Time:")
                {
                    FrameSpan = float.Parse(words[2]);
                    isFrameSpan = true;
                }
                else if (words[0] == "Frame" && words[1] == "Time" && words[2] == ":")
                {
                    FrameSpan = float.Parse(words[3]);
                    isFrameSpan = true;
                }
            }
            else
            {
                //同一フレーム間
                int wordIndex = 0;
                for (int i = 0; i < _nodeOrder.Count; i++)
                {
                    var node = _nodeOrder[i];

                    BvhVec3 pos = new BvhVec3();
                    BvhVec3 rot = new BvhVec3();
                    for (int ch = 0; ch < node.Channnels.Count; ch++)
                    {

                        if (words[wordIndex] == "") return true; // 仮処理 モーション部が終わった後に改行とか入ってたとりあえず無視する。

                        var channnel = node.Channnels[ch];
                        float value = float.Parse(words[wordIndex]);
                        switch (channnel)
                        {
                            case BvhNode.Channel.Xposition: pos.X = value; break;
                            case BvhNode.Channel.Yposition: pos.Y = value; break;
                            case BvhNode.Channel.Zposition: pos.Z = value; break;
                            case BvhNode.Channel.Xrotation: rot.X = value; break;
                            case BvhNode.Channel.Yrotation: rot.Y = value; break;
                            case BvhNode.Channel.Zrotation: rot.Z = value; break;
                        }
                        wordIndex++;
                    }
                    node.SetMotionPos(motionNum, pos);
                    node.SetMotionRot(motionNum, rot);
                }
                motionNum++;
            }

            return true;
        }
        #endregion

        #region Save Bvh data
        public void Save(string filepath)
        {
            if (filepath == null) return;
            using (StreamWriter wr = new StreamWriter(filepath))
            {
                writeHierarchy(wr, _root);
                writeMotion(wr);
            }
        }

        private void writeHierarchy(StreamWriter wr, BvhNode root)
        {
            wr.WriteLine("HIERARCHY");
            wr.WriteLine("ROOT {0}", root.Name);
            wr.WriteLine("{");
            wr.WriteLine("\t" + getStringOffset(root.Offset));
            wr.WriteLine("\t" + getStringChannel(root.Channnels));

            if (root.Nodes.Count == 0)
            {
                wr.WriteLine("\tEnd Site");
                wr.WriteLine("\t{");
                wr.WriteLine("\t\t" + getStringOffset(root.Offset));
                wr.WriteLine("\t}");
            }
            else
            {
                foreach (var node in root.Nodes)
                    writeJoint(wr, node, 1);
            }
            wr.WriteLine("}");
        }
        private void writeJoint(StreamWriter wr, BvhNode bvhNode, int layer)
        {
            // Add tab showing hierarchy
            string tab = "";
            for (int i = 0; i < layer; i++)
                tab += "\t";

            // Export joint info
            wr.WriteLine(tab + getStringJointName(bvhNode.Name));
            wr.WriteLine(tab + "{");
            wr.WriteLine(tab + "\t" + getStringOffset(bvhNode.Offset));
            wr.WriteLine(tab + "\t" + getStringChannel(bvhNode.Channnels));


            if (bvhNode.Nodes.Count == 0)
            {
                //There is no child - terminal joint
                wr.WriteLine(tab + "\tEnd Site");
                wr.WriteLine(tab + "\t{");
                wr.WriteLine(tab + "\t\t" + getStringOffset(new BvhVec3(0, 0, 0)));
                wr.WriteLine(tab + "\t}");
            }
            else
            {
                foreach (var child in bvhNode.Nodes)
                {
                    writeJoint(wr, child, layer + 1);
                }
            }
            wr.WriteLine(tab + "}");
        }
        private void writeMotion(StreamWriter wr)
        {
            // HEADER
            wr.WriteLine("MOTION");
            wr.WriteLine("Frames: {0}", FrameNum);
            wr.WriteLine("Frame Time: {0}", FrameSpan);

            for (int frame = 0; frame < FrameNum; frame++)
            {
                string buffer = "";
                // Use the order of the nodes acquired at the time of reading
                foreach (var node in _nodeOrder)
                {
                    // It is necessary to write in order of channel
                    foreach (var channel in node.Channnels)
                    {
                        var pos = node.GetMotionPos(frame);
                        var rot = node.GetMotionRot(frame);

                        switch (channel)
                        {
                            case BvhNode.Channel.Xposition: buffer += pos.X.ToString(); break;
                            case BvhNode.Channel.Yposition: buffer += pos.Y.ToString(); break;
                            case BvhNode.Channel.Zposition: buffer += pos.Z.ToString(); break;
                            case BvhNode.Channel.Xrotation: buffer += rot.X.ToString(); break;
                            case BvhNode.Channel.Yrotation: buffer += rot.Y.ToString(); break;
                            case BvhNode.Channel.Zrotation: buffer += rot.Z.ToString(); break;
                        }
                        buffer += " ";
                    }
                }// end BvhNode

                wr.WriteLine(buffer); //After reading one frame, output
            }// end frame
            wr.WriteLine(); // Line feed
        }

        private string getStringJointName(string name)
        {
            return "JOINT " + name;
        }
        private string getStringOffset(BvhVec3 offset)
        {
            string buffer = "";
            buffer += "OFFSET ";
            buffer += offset.X.ToString();
            buffer += " ";
            buffer += offset.Y.ToString();
            buffer += " ";
            buffer += offset.Z.ToString();

            return buffer;
        }
        private string getStringChannel(List<BvhNode.Channel> channels)
        {
            string buffer = "CHANNELS " + channels.Count.ToString() + " ";

            foreach (var c in channels)
            {
                switch (c)
                {
                    case BvhNode.Channel.Xposition: buffer += "Xposition "; break;
                    case BvhNode.Channel.Yposition: buffer += "Yposition "; break;
                    case BvhNode.Channel.Zposition: buffer += "Zposition "; break;
                    case BvhNode.Channel.Xrotation: buffer += "Xrotation "; break;
                    case BvhNode.Channel.Yrotation: buffer += "Yrotation "; break;
                    case BvhNode.Channel.Zrotation: buffer += "Zrotation "; break;
                }
            }
            return buffer;
        }
        #endregion

        public BvhNode GetRootNode()
        {
            return _root;
        }
        public BvhNode GetNode(string name)
        {
            return _nodeDic.ContainsKey(name) ? _nodeDic[name] : null;
        }
        public List<BvhNode> GetNodeList()
        {
            return _nodeOrder;

        }


    }
}
