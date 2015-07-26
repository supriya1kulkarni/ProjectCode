using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.IO;
/*Author:Supriya Kulkarni
  ClassFileName: DecisionID3.cs
  Purpose:  This class file has functions to build the decision tree */
namespace MachineLearningID3
{
    public class DecisionID3
    {

        private DataTable tuples;
        private int TotalPositives = 0;
        private int TotalTuples = 0;
        private string resultClass = "result";
        private double Entropy = 0.0;


        /*This function is used to count positive classes i.e belong to class with label 1*/

        private int countPositiveClass(DataTable samples)
        {
            int result = 0;

            foreach (DataRow aRow in samples.Rows)
            {
                if (aRow[resultClass].ToString() == "1")
                    result++;
            }

            return result;
        }
        /*This function is used to calculate the entropy*/
        private double calcEntropy(int positives, int negatives)
        {
            int total = positives + negatives;
            if (total > 0)
            {
                double ratioPos = (double)positives / total;
                double ratioNeg = (double)negatives / total;

                if (ratioPos != 0d)
                    ratioPos = -(ratioPos) * System.Math.Log(ratioPos, 2);
                if (ratioNeg != 0d)
                    ratioNeg = -(ratioNeg) * System.Math.Log(ratioNeg, 2);

                double result = ratioPos + ratioNeg;

                return result;
            }
            else
            {
                return 0d;
            }
        }

        private void getClassSeperationValues(DataTable samples, Attribute attribute, string value, out int positives, out int negatives)
        {
            positives = 0;
            negatives = 0;

            foreach (DataRow aRow in samples.Rows)
            {
                if (((string)aRow[attribute.AttributeName] == value))
                    if (aRow[resultClass].ToString() == "1")
                        positives++;
                    else
                        negatives++;
            }
        }
        /*This function is used to calculate the info gain*/
        private double Infogain(DataTable samples, Attribute attribute)
        {
            string[] values = attribute.values;
            double sum = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                int positives, negatives;

                positives = negatives = 0;

                getClassSeperationValues(samples, attribute, values[i], out positives, out negatives);


                double entropy = calcEntropy(positives, negatives);

                sum += -(double)(positives + negatives) / TotalTuples * entropy;


            }
            return Entropy + sum;
        }
        /*This function is used to find the best attribute to split based on information gain obtained*/
        private Attribute getSplittingAttribute(DataTable samples, Attribute[] attributes)
        {
            double maxGain = 0.0;
            Attribute result = null;

            foreach (Attribute attribute in attributes)
            {
                double tempgain = Infogain(samples, attribute);

                if (tempgain > maxGain)
                {
                    maxGain = tempgain;
                    result = attribute;

                }
            }
            return result;
        }
        /*This function is used to check if all tuples belong to class with label 1*/
        private bool tuplePositiveTest(DataTable samples, string resultLabel)
        {
            foreach (DataRow row in samples.Rows)
            {
                if ((row[resultLabel].ToString()) == "0")
                    return false;
            }

            return true;
        }
        /*This function is used to check if all tuples belong to class with label 0*/
        private bool tupleNegativeTest(DataTable samples, string resultLabel)
        {
            foreach (DataRow row in samples.Rows)
            {
                if ((row[resultLabel].ToString()) == "1")
                    return false;
            }

            return true;
        }
        /*This function is used to get distinct values of the resulting class label*/
        private ArrayList getDistinctValues(DataTable tuples, string resultLabel)
        {
            ArrayList UniqueValues = new ArrayList(tuples.Rows.Count);

            foreach (DataRow row in tuples.Rows)
            {
                if (UniqueValues.IndexOf(row[resultLabel].ToString()) == -1)
                    UniqueValues.Add(row[resultLabel].ToString());
            }

            return UniqueValues;
        }

        /*This function is used to get the most frequently occuring class label value in the given tuples*/
        private object getMostCommonValue(DataTable samples, string resultLabel)
        {
            ArrayList UniqueValues = getDistinctValues(samples, resultLabel);
            int[] countValue = new int[UniqueValues.Count];

            foreach (DataRow row in samples.Rows)
            {
                int index = UniqueValues.IndexOf(row[resultLabel]);
                countValue[index]++;
            }

            int MIndex = 0;
            int MCount = 0;

            for (int i = 0; i < countValue.Length; i++)
            {
                if (countValue[i] > MCount)
                {
                    MCount = countValue[i];
                    MIndex = i;
                }
            }

            return UniqueValues[MIndex];
        }

        /*This function is used to construct the tree amd is called recursively*/
        private TreeNode constructTree(DataTable samples, string resultLabel, Attribute[] attributes, string filename, ArrayList attributenames)
        {
            if (tuplePositiveTest(samples, resultLabel) == true) /* check if all tuples belong to class label 1*/
                return new TreeNode(new Attribute("1"));

            if (tupleNegativeTest(samples, resultLabel) == true) /* check if all tuples belong to class label 1*/
                return new TreeNode(new Attribute("0"));


            TotalTuples = samples.Rows.Count;
            resultClass = resultLabel;
            TotalPositives = countPositiveClass(samples);
            int mnegative = TotalTuples - TotalPositives;
            /*Below are the conditions that check when attribute set is empty or tuples are over etc.,*/
            if (attributes.Length == 0 && TotalPositives == mnegative)
                return new TreeNode(new Attribute(getMostCommonValue(samples, resultLabel)));
            else if (attributes.Length == 0 && TotalPositives == mnegative)
                return new TreeNode(new Attribute(getMostCommonValue(getDataTable(filename, attributenames), resultLabel)));


            else if (samples.Rows.Count == 0)
                return new TreeNode(new Attribute(getMostCommonValue(getDataTable(filename, attributenames), resultLabel)));



            Entropy = calcEntropy(TotalPositives, TotalTuples - TotalPositives);
            /*To find the best attribute*/
            Attribute bestAttribute = getSplittingAttribute(samples, attributes);
            if (bestAttribute == null && TotalPositives == mnegative)
            {

                return new TreeNode(new Attribute(getMostCommonValue(getDataTable(filename, attributenames), resultLabel)));
            }
            else if (bestAttribute == null && TotalPositives != mnegative)
            {
                return new TreeNode(new Attribute(getMostCommonValue(samples, resultLabel)));
            }
            TreeNode root = new TreeNode(bestAttribute);

            DataTable aSample = samples.Clone();
            bestAttribute.postives = TotalPositives;
            bestAttribute.negatives = mnegative;
            /*Loop through all possible attribute values to split based on the above best attribute obtained */
            foreach (string value in bestAttribute.values)
            {



                aSample.Rows.Clear();

                DataRow[] rows = samples.Select(bestAttribute.AttributeName + " = " + "'" + value + "'");

                foreach (DataRow row in rows)
                {
                    aSample.Rows.Add(row.ItemArray);
                }
                ArrayList aAttributes = new ArrayList(attributes.Length - 1);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].AttributeName != bestAttribute.AttributeName)
                        aAttributes.Add(attributes[i]);
                }




                DecisionID3 dc3 = new DecisionID3();
                TreeNode child = dc3.MainTree(aSample, resultLabel, (Attribute[])aAttributes.ToArray(typeof(Attribute)), filename, attributenames);
                root.AddNode(child, value);

            }

            return root;
        }


        public TreeNode MainTree(DataTable samples, string targetAttribute, Attribute[] attributes, string filename, ArrayList attributenames)
        {
            tuples = samples;
            return constructTree(tuples, targetAttribute, attributes, filename, attributenames);
        }

        /* This function reads data from the train/test file and converts it to datatable*/
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
                        var eachword = templine.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var w in eachword)
                        {
                            val.Add(w);
                        }
                        String[] arraytmp = new String[val.Count];
                        for (int i = 0; i < val.Count; i++)
                        {
                            arraytmp[i] = val[i].ToString();
                        }

                        result.Rows.Add(arraytmp);


                    }

                    val.Clear();
                }


                return result;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occured " + ex.Message);
                return result;
            }
        }
    }
}

    

