using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace GenieWin8
{
    class UtilityTool
    {
        Dictionary<string, string> responseDic;
        private string dicKey;
        public UtilityTool()
        { }
        public string SubString(string source ,string start, string end)
        {
            int s = source.IndexOf(start);
            int e = source.IndexOf(end);
            return source.Substring(s+start.Length, e - s - end.Length +1);
        }

        /////遍历xml/////////
        public Dictionary<string,string> TraverseXML(string xml)
        {
            responseDic = new Dictionary<string,string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList xnl = xmlDoc.DocumentElement.ChildNodes;
            string root = xmlDoc.FirstChild.NextSibling.NodeName;
            foreach(IXmlNode xn in xnl)
            {
                IXmlNode child = xn.FirstChild;
                if (child != null)
                {
                    NodeOperate(child, root);
                }
            }
            return responseDic;

        }

        private void NodeOperate(IXmlNode xn,string root)
        {
            
            if (xn.HasChildNodes())
            {
                dicKey = xn.NodeName;
                System.Diagnostics.Debug.WriteLine(xn.NodeName + "\n");
                //System.Diagnostics.Debug.WriteLine("\n");
                IXmlNode childNode = xn.FirstChild;
                
                NodeOperate(childNode,root);

            }
            else
            {
                
                //System.Diagnostics.Debug.WriteLine(xn.NodeName + "\n");
                System.Diagnostics.Debug.WriteLine(xn.InnerText);
                //System.Diagnostics.Debug.WriteLine("\n");
                string dicValue = xn.InnerText.Trim() ;
                if (dicValue != "")
                {
                    responseDic.Add(dicKey, dicValue);
                }
                if (xn.NextSibling != null)
                {

                    NodeOperate(xn.NextSibling,root);
                }
                else
                {
                    int flag = 0;
                    while (xn.NextSibling == null)
                    {
                        if (xn.NodeName == root)
                        {
                            flag = 1;
                            break;
                        }
                        else
                        {
                            xn = xn.ParentNode;
                        }

                    }
                    if (flag == 0)
                    {
                        NodeOperate(xn.NextSibling,root);
                    }
                    else if (flag == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("End");
                    }
                }
            }
        }
                
    }
}
