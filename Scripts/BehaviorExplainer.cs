using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBehaviorTreeEditor.Scripts
{
    public class BehaviorExplainer
    {
        private static StringBuilder sb = new StringBuilder();

        public static string ExplainText(BehaviorNode node)
        {
            sb.Clear();
            if(node.title.Text == "Sequence")
            {

                if (node.children.Count == 1)
                    sb.AppendFormat("Returns SUCCESS if {0} is SUCCESS", node.children[0].title.Text);
                else if (node.children.Count > 1)
                {
                    for(int i = 0; i < node.children.Count; i++)
                    {
                        if (node.children[i].title.Text == "Sequence")
                        {
                            if(sb.Length > 0)
                            {
                                sb.Append("and then returns SUCCESS when all of the children returns SUCCESS ");
                            }
                            else
                            {
                                sb.Append("Returns SUCCESS when all of the children returns SUCCESS ");
                            }
                        }
                        else if(node.children[i].title.Text == "Selector")
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("and then returns SUCCESS when first child returns SUCCESS ");
                            }
                            else
                            {
                                sb.Append("returns SUCCESS for first node that returns SUCCESS");
                            }
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.AppendFormat("and if it returns SUCCESS then calls {0} ", node.children[i].title.Text);
                            }
                            else
                            {
                                sb.AppendFormat("Calls {0} ", node.children[0].title.Text);

                            }
                        }
                    }
                }
            }
            else if (node.title.Text == "Selector")
            {

                if (node.children.Count == 1)
                    sb.AppendFormat("Returns SUCCESS if {0} is SUCCESS", node.children[0].title.Text);
                else if (node.children.Count > 1)
                {
                    for (int i = 0; i < node.children.Count; i++)
                    {
                        if (node.children[i].title.Text == "Sequence")
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("or returns SUCCESS if all of the children returns SUCCESS ");
                            }
                            else
                            {
                                sb.Append("Returns SUCCESS when all of the children returns SUCCESS ");
                            }
                        }
                        else if (node.children[i].title.Text == "Selector")
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("or then returns SUCCESS when first child returns SUCCESS ");
                            }
                            else
                            {
                                sb.Append("returns SUCCESS for first node that returns SUCCESS");
                            }
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.AppendFormat("or calls {0} for SUCCESS ", node.children[i].title.Text);
                            }
                            else
                            {
                                sb.AppendFormat("Calls {0} ", node.children[0].title.Text);

                            }
                        }
                    }
                }
            }
            else
            {
                if (node.children.Count > 0)
                {
                    return "You should use Selector or Sequence to have children.";
                }
            }

            if (node.children.Count == 0)
                return "Calls itself";
                

            return sb.ToString();
        }

    }
}
