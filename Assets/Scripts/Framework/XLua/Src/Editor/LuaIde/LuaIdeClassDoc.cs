using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;


    public class LuaIdeClassDocInfo{
        public string name;
        public string doc;
        public Dictionary<string,string> paraminfo = new Dictionary<string,string>();
    }
    public class LuaIdeClassDoc
    {
        public static List<LuaIdeClassDocInfo> infos;
        public static void checkDoc()
        {
            infos = new List<LuaIdeClassDocInfo>();
            
            string path = Application.dataPath + "/doc.xml";

            if (!File.Exists(path))
            {
                return;
            }
            string xx = "1";

            //创建xml文档
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
            xml.Load(XmlReader.Create((path), set));
            //得到objects节点下的所有子节点
            XmlNodeList xmlNodeList = xml.SelectSingleNode("doc").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (xl1.Name == "members")
                {
                    //继续遍历id为1的节点下的子节点
                    foreach(XmlElement xl2 in xl1.ChildNodes)
                    {
                        LuaIdeClassDocInfo info = new  LuaIdeClassDocInfo();

                        string name = xl2.GetAttribute("name");

                        if (name.IndexOf(":") > -1) {
                            name = name.Split(':')[1];
                        }
                        if (name.IndexOf('(') > -1) {
                            name = name.Split('(')[0];
                        }
                        
                        info.name = name;
                        info.doc = "";

                        infos.Add(info);
                        try
                        {
                            foreach (XmlElement xl3 in xl2.ChildNodes)
                            {
                                if (xl3.Name == "summary")
                                {
                                    string doc1 = "";
                                    string doc = xl3.InnerText.Trim();
                                    if (doc.IndexOf('\n') > -1)
                                    {
                                        string[] docs = doc.Split('\n');
                                        for(int i = 0;i < docs.Length;i++)
                                        {
                                            string str = docs[i];
                                            if (i == docs.Length - 1)
                                            {
                                                doc1 += "\t" + str.Trim() ;
                                            }
                                            else {
                                                doc1 += "\t" + str.Trim() + "\n";
                                            }
                                            
                                        }
                                        info.doc = doc1;
                                    }
                                    else {
                                        info.doc = "\t"+ doc;
                                    }

                                }
                                else if (xl3.Name == "param")
                                {
                                    string pdoc = xl3.InnerText.Trim();
                                    if (pdoc.IndexOf('\n') > -1) {
                                        string[] pdocs = pdoc.Split('\n');
                                        pdoc = "";
                                        for (int i = 0; i < pdocs.Length; i++)
                                        {
                                            pdoc += pdocs[i].Trim() + " ";
                                        } 


                                    }
                                    info.paraminfo.Add(xl3.GetAttribute("name"), pdoc);
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            int ss = 1;
                        }
                        
                    }

                }
            }
           

        }
        public static LuaIdeClassDocInfo getLuaIdeClassDocInfo(string key)
        {
            foreach (LuaIdeClassDocInfo info in infos)
            {
                if (info.name == key)
                {
                    return info;
                }
            }
            return null;
        }
        public static string getDoc(string key)
        {
            foreach(LuaIdeClassDocInfo info in infos){
                if(info.name == key){
                    return info.doc;
                }
            }
            return "";
        }




    }


