using System;
using System.Collections.Generic;
using System.Linq;

using (Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser("test.csv"))
{
    int numRecords = 0;     //Counts the total number of records in the given CSV
    int largestSum = 0;     //To store our largest sum of Val1 and Val2
    int count = 0;          //Counts which field we are in
    int colNum = 0;         //Counts wich column we are in
    int allVal3Length = 0;  //The total combined lengths of Val3
    int val3AvgLength = 0;  //The average of the Val3 lengths
    string largestRecord = null;    //The GUID of our largest sum
    List<string> records = new List<string>();  //A list of all of our GUID
    List<int> val1List = new List<int>();       //A list of all Val1
    List<int> val2List = new List<int>();       //A list of all Val2
    List<string> val3List = new List<string>(); //A list of all Val3
    List<int> sums = new List<int>();           //A list of all the sums of Val1 and Val2
    List<int> val3Lengths = new List<int>();    //A list of all Val3 lengths
    parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
    parser.SetDelimiters(","); //Parses the CSV by seperating each value by a ","
    while (!parser.EndOfData) 
    {
        string[] fields = parser.ReadFields();
        foreach (string field in fields) 
        {
            //Will loop until every value "field" in "fields" is read.
            //Each field is the value of the data in the given CSV
            count++;

            if (count > 4)
            {
                //Starts once we are past the headers in the CSV
                colNum++;
                //colNum is used to decide if we are storing the read value into
                //our GUID list, Val1 list, Val2 list, or Val3 list

                if (colNum == 1) records.Add(field);
                else if (colNum == 2) val1List.Add(Int32.Parse(field));
                else if (colNum == 3) val2List.Add(Int32.Parse(field));
                else if (colNum == 4) 
                {
                    val3List.Add(field);
                    colNum = 0;
                    //Everytime that we read Val3, we reset the colNum so that we can store
                    //the rest of our data.
                }
            }
        }
    }

    numRecords = (count - 4) / 4;   //The total number of records is calculated by subtracting
                                    //the 4 header values in the CSV from count, then dividing
                                    //that result by 4 (the total amount of data per record).

    for (int i = 0; i < numRecords; i++) 
    {
        sums.Add((val1List[i] + val2List[i]));  //Adds every sum and stores it in our list
        val3Lengths.Add(val3List[i].Length);    //Adds the length of every Val3 to our list
        allVal3Length += val3Lengths[i];        //Adds the total length of all Val3
    }

    for (int i = 0; i < numRecords; i++)
    {
        if (sums[i] > largestSum)
        {
            largestSum = sums[i];       //Stores the largest sum of Val1 and Val2
            largestRecord = records[i]; //Stores the GUID for the largest sum
        }
    }

    List<string> duplicateRecords = new List<string>(); //Stores all duplicate GUID values
    for (int i = 0; i < numRecords; i++)
    {
        for (int j = i + 1; j < numRecords; j++)
        {
            if (records[i].Equals(records[j]))
            {
                //If GUID i equals GUID j, then we add the value to our duplicate records list
                duplicateRecords.Add(records[j]);
            }
        }
    }
    for (int i = 0; i < duplicateRecords.Count; i++)
    {
        for (int j = i + 1; j < duplicateRecords.Count; j++)
        {
            if (duplicateRecords[i].Equals(duplicateRecords[j]))
            {
                //If a GUID shows up more than once, we remove the repeated values
                //So that we only print the value once.
                duplicateRecords.RemoveAt(j);
                j--;
            }
        }
    }
    Console.WriteLine("Number of records: " + numRecords); //Prints the total number of records
    Console.WriteLine("Largest sum and its GUID: " + largestSum + ", " + largestRecord); //Prints our largest sum and its GUID
    Console.WriteLine("Duplicate Records: " + String.Join(", ", duplicateRecords)); //Prints all of our duplicate GUID values
    val3AvgLength = allVal3Length / numRecords; //Calculates our Val3 length average by taking the total length of all our Val3 values and divides it by the total number of records.
    Console.WriteLine("Val3 Average Length: " + val3AvgLength); //Prints our average Val3 length

    List<string> isOverAvg = new List<string>(); //Stores "Y" or "N" if our Val3 is over or under the average length
    for (int i = 0; i < numRecords; i++)
    {
        if (val3Lengths[i] > val3AvgLength) isOverAvg.Add("Y");
        else isOverAvg.Add("N");
    }

    List<string> isDuplicate = new List<string>(); //Stores "Y" or "N" if our GUID is a duplicate value
    for (int i = 0; i < numRecords; i++)
    {
        isDuplicate.Add("N"); //Initially sets all elements to "N"
    }
    for (int j = 0; j < duplicateRecords.Count(); j++)
    {
        for (int i = 0; i < numRecords; i++)
        {
            if (records[i].Equals(duplicateRecords[j]))
            {
                //If the GUID is a duplicate, then we store "Y" in our duplicate list
                isDuplicate[i] = "Y";
            }
        }
    }

    using (var csv = new StreamWriter("Output.csv"))
    {
        var line = string.Format("{0}, {1}, {2}, {3}", "GUID", "Val1 + Val2", "IsDuplicateGuid (Y or N)", "Val3 > Avg (Y or N)");
        //line is used as the format to print each row in the CSV
        csv.WriteLine(line);
        //Writes the headers for each column
        csv.Flush();
        for (int i = 0; i < numRecords; i++)
        {
            line = string.Format("{0}, {1}, {2}, {3}", records[i], sums[i], isDuplicate[i], isOverAvg[i]);
            //Loops through all of the lists to print each value for our CSV
            csv.WriteLine(line);
            csv.Flush();
        }
    }
    
}
