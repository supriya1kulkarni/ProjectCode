using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Collections;
/*Author:Supriya Kulkarni
  ClassFileName:Program.cs
  Purpose:This is the main classfile,has the main function which takes the training file and test file as inputs and prints
          the decision tree , displays the accuracy for the training and testing file.   
 */
namespace MachineLearningID3
{




    class Misc
    {
        /*This function is used to print the decision tree*/
        public static void printNode(TreeNode root, string tabs)
        {

            if (root.totalCountOfChilds == 1)
            {
                Console.Write(root.attribute);
                Console.Write(Environment.NewLine);
            }
            else
            {

                Console.Write(tabs + "|" + root.attribute);

            }
            if (root.attribute.values != null)
            {
                for (int i = 0; i < root.attribute.values.Length; i++)
                {

                    if (i > 0)
                    {
                        Console.Write(tabs + "|" + root.attribute + "=" + root.attribute.values[i] + ":");
                    }
                    else
                    {
                        Console.Write("=" + root.attribute.values[i] + ":");
                    }
                    TreeNode childNode = root.getBranchChild(root.attribute.values[i]);
                    if (childNode.totalCountOfChilds > 1)
                    {
                        Console.Write(Environment.NewLine);
                        printNode(childNode, "| \t" + tabs);
                    }
                    else
                    {
                        printNode(childNode, "\t" + tabs);
                    }

                }

            }


        }
        /* The function is used to traverse the decision tree*/
        public static void TraverseMain(TreeNode node, string filename, ArrayList attributenames, string filetype, ref DataTable result, int colNumber)
        {
            DataTable dt = getDataTable(filename, attributenames);
          
            string val = string.Empty;

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                val = string.Empty;
                Traverse(node, dt.Rows[j], ref val);
                result.Rows[j][colNumber] = val;


            }

        }
        public static void Traverse(TreeNode node, DataRow dt, ref string val)
        {
            string currentval = string.Empty;



            if (node.attribute.values != null)
            {
                currentval = dt[node.attribute.ToString()].ToString();
                bool result = CheckForMissingValue(node.attribute.values, currentval);
                if (result == true)
                {
                    if (node.attribute.postives >= node.attribute.negatives)
                    {
                        val = "1";
                        return;
                    }
                    else if (node.attribute.postives < node.attribute.negatives)
                    {
                        val = "0";
                        return;
                    }
                }
                else
                {

                    for (int i = 0; i < node.attribute.values.Length; i++)
                    {
                        currentval = dt[node.attribute.ToString()].ToString();
                        if (currentval == node.attribute.values[i])
                        {
                            TreeNode childNode = node.getBranchChild(node.attribute.values[i]);

                            Traverse(childNode, dt, ref val);

                        }
                    }

                }
            }

            else
            {

                val = node.attribute.aLabel.ToString();
                return;
            }

        }
        /* This function is used to calculate accuracy */
        static void CalculateAccuracy(DataTable maintbl, DataTable calctbl, string filetype)
        {
            int correct = 0;
            double accuracy = 0.0;
            for (int i = 0; i < maintbl.Rows.Count; i++)
            {

                if (maintbl.Rows[i]["result"].ToString().Equals(calctbl.Rows[i]["result"].ToString()))
                {
                    correct++;
                }
            }

            accuracy = (((double)correct / (double)maintbl.Rows.Count) * 100);
            Console.WriteLine("Accuracy on " + filetype + " (" + maintbl.Rows.Count + " instances) is " + Math.Round(accuracy, 2) + "%.");
        }

        /* This functon is used to check for missing values*/
        static public bool CheckForMissingValue(string[] values, string checkforvalue)
        {
            foreach (string wrd in values)
            {
                if (wrd.Equals(checkforvalue.ToString().Trim()))
                {
                    return false;
                }
            }
            return true;
        }

        /*This function will read the data from the fle and convert it into a DataTable*/
        static DataTable getDataTable(string filename, ArrayList attributenames)
        {


            DataTable result = new DataTable("samples");
            try
            {
                var lines = File.ReadAllLines(filename);



                string templine;

                ArrayList val = new ArrayList();
                foreach (string attrname in attributenames)
                {
                    DataColumn column = result.Columns.Add(attrname);
                    column.DataType = typeof(string);

                }


                DataColumn res = result.Columns.Add("result");
                res.DataType = typeof(string);
                foreach (var line in lines)
                {
                    if (line != lines.First())
                    {
                        templine = line.Replace('\t', ' ');
                        var eachword = templine.Split(' ');

                        foreach (var w in eachword)
                        {
                            val.Add(w);
                        }

                        String[] arrayTemp = new String[val.Count];
                        for (int i = 0; i < val.Count; i++)
                        {
                            arrayTemp[i] = val[i].ToString();
                        }

                        result.Rows.Add(arrayTemp);

                    }

                    val.Clear();
                }



                return result;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                return result;
            }

        }
        /*This function will read the attribute names from the file*/
        static ArrayList getAttributeNames(string filename)
        {
            ArrayList attributeNames = new ArrayList();
            int temp;
            try
            {
                var flines = File.ReadLines(filename).Take(1).ToArray();

                flines[0] = flines[0].Replace('\t', ' ');
                var wrd = flines[0].Split(' ');
                foreach (var w in wrd)
                {

                    bool res = int.TryParse(w, out temp);
                    if (res == false)
                    {
                        attributeNames.Add(w);
                    }
                }
                return attributeNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured " + ex.Message);
                return attributeNames;
            }
        }
        /*This function will retrieve the possible values for the attributes*/
        static Attribute[] getList(DataTable result, string[] attributenames)
        {
            DataView view = new DataView(result);
            DataTable distinctValues = new DataTable();
            ArrayList possiblevalues = new ArrayList();
            Attribute[] attributes = new Attribute[6];
            int k = 0;
            for (int i = 0; i < (result.Columns.Count - 1); i++)
            {
                distinctValues = view.ToTable(true, result.Columns[i].ToString());
                foreach (DataRow r in distinctValues.Rows)
                {
                    possiblevalues.Add(r[0].ToString());
                }
                Attribute a = new Attribute(attributenames[i].ToString(), possiblevalues.ToArray(typeof(string)) as string[]);
                possiblevalues.Clear();
                distinctValues.Clear();
                attributes[k++] = a;
            }
            return attributes;
        }
        static DataTable GetRandomSamples(DataTable samples)
        {
            DataTable newTable;
            newTable = samples.Clone();
            Random rnd = new Random();
            for (int i = 0; i < samples.Rows.Count; i++)
            {
                int k = rnd.Next(samples.Rows.Count);
                newTable.ImportRow(samples.Rows[k]);
            }
            return newTable;

        }

        static void CalcuateMajorityVotes(ref DataTable result,string filename,ArrayList attributenames){
            int positiveCount, negativeCount;
            positiveCount = negativeCount = 0;
            DataTable finalresult = new DataTable();
            DataColumn res = finalresult.Columns.Add("result");
            res.DataType = typeof(string);
            for (int i = 0; i < result.Rows.Count; i++)
            {
                for (int j = 0; j < result.Columns.Count; j++)
                {
                    if (result.Rows[i][j].ToString() == "1")
                    {
                        positiveCount++;
                    }
                    else
                    {
                        negativeCount++;
                    }
                }
                if (positiveCount >= negativeCount)
                {
                    finalresult.Rows.Add("1");
                }
                else
                {
                    finalresult.Rows.Add("0");
                }
                positiveCount = negativeCount = 0;
            }

            DataTable dt = getDataTable(filename, attributenames);
            CalculateAccuracy(dt, finalresult, "test");


        }
        [STAThread]
        /*This is the main function which will read the fle names from the commnadline and call the decision tree
         * and accuracy calculation functions*/
        static void Main(string[] args)
        {
            string trainfile, testfile;
            DataTable result = new DataTable();
            for (int l = 0; l < 10; l++)
            {
                result.Columns.Add(l.ToString(), typeof(string));
            }
           
                Console.WriteLine("Enter the training file name");

            trainfile = Console.ReadLine().ToString().Trim();

            Console.WriteLine("Enter the test file name");
            testfile = Console.ReadLine().ToString().Trim();

            ArrayList attributenames = getAttributeNames(trainfile);
            DataTable samples = getDataTable(trainfile, attributenames);
            for (int m = 0; m < samples.Rows.Count; m++)
            {
                result.Rows.Add();
            }
            string[] array = attributenames.ToArray(typeof(string)) as string[];
            for (int k = 0; k < 10; k++)
            {
                DataTable random = GetRandomSamples(samples);
                Attribute[] attributes = getList(random, array);
                DecisionID3 id3 = new DecisionID3();
                TreeNode root = id3.MainTree(random, "result", attributes, trainfile, attributenames);
              

                TraverseMain(root, testfile, attributenames, "Test File", ref result, k);
                

            }
            CalcuateMajorityVotes(ref result,testfile,attributenames);
            Console.ReadLine();
        }
    }
    }

