using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
/*Author:Supriya Kulkarni
  ClassFileName:TreeNode.cs
  Purpose:This class file is used to define the Tree and some yhe important operations like adding nodes to the tree etc.,
          The data structure used to implement the tree is ArrayList of ArrayList */
namespace MachineLearningID3
{
   public class TreeNode
    {
        
            private ArrayList tChilds = null;
            private Attribute tAttribute;
       /*The constructoe that adds the child to the trenode*/
            public TreeNode(Attribute attribute)
            {
                if (attribute.values != null)
                {
                    tChilds = new ArrayList(attribute.values.Length);
                    for (int i = 0; i < attribute.values.Length; i++)
                        tChilds.Add(null);
                }
                else
                {
                    tChilds = new ArrayList(1);
                    tChilds.Add(null);
                }
                tAttribute = attribute;
            }
       /*The function adds a node to the Tree*/
            public void AddNode(TreeNode treeNode, string ValueName)
            {
                int index = tAttribute.indexValue(ValueName);
                tChilds[index] = treeNode;
            }

            public int totalCountOfChilds
            {
                get
                {
                    return tChilds.Count;
                }
            }
       /*The function is used to get the Child at a specified index*/
            public TreeNode getTreeChild(int index)
            {
                return (TreeNode)tChilds[index];
            }

            public Attribute attribute
            {
                get
                {
                    return tAttribute;
                }
            }

            /*The function is used to get the Child based on branch name*/
            public TreeNode getBranchChild(string branchName)
            {
                int index = tAttribute.indexValue(branchName);
                return (TreeNode)tChilds[index];
            }
        }

    
}
