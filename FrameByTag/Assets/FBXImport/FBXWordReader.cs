using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;



public class FBXWordReader : MonoBehaviour
{
    public char[] word;
    public bool endReached;

    public char currentChar;
    private int currentPosition = 0;
    private int maxPosition = 0;

    private StreamReader reader;
    private int bufferSize;
    private char[] buffer;
    private string path;

    char[] white = new char[] { ' ', '\n', '\t', '\r' };
    char[] newLn = new char[] { '\n', '\r' };
    char[] quote = new char[] { '\"' };
    char[] comma = new char[] { ',' };
    char[] mlArr = new char[] { '\t', '}' };

    public FBXWordReader(string path)
    {
        this.reader = reader;
        this.path = path;
        //TextReader woof = reader;

       
    }

    public void FindPropertiesByName(string propertyName)
    {
        
    }


    public void GetVertices()
    {

    }

    public void ReadLine()
    {
        using (var woof = File.OpenRead(path))
        {
            var line = woof.ReadByte();
            var bytes = File.ReadAllBytes(path);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 26; i++)
            {
                sb.Append(bytes[i] + " ");
            }
            //Debug.Log(Encoding.Convert(Encoding.ASCII, Encoding.Default, sb));
        }

        using (var woof = File.OpenText(path))
        {
            var lines = File.ReadAllLines(path, Encoding.ASCII);
            int n = 1;
            //Debug.Log(Encoding.Convert(Encoding.ASCII,Encoding.Default, lines[n]));
            //foreach (var line in lines)
            //{
            //    Debug.Log(line.ToString());
            //}
        }


    }
    public void ReadAllLines()
    {
        //using (var woof = File.OpenText(path))
        //{
        //    var lines = File.ReadAllLines(path, System.Text.Encoding.ASCII);
        //    foreach (var line in lines)
        //    {
        //        Debug.Log(line.ToString());
        //    }
        //}

        string[] fileBytes = File.ReadAllLines(path);
        StringBuilder sb = new StringBuilder();

        foreach (var b in fileBytes)
        {
            sb.Append(b.ToString());
        }

        Debug.Log(sb);
        //byte[] buffer = new byte[512];
        //try
        //{
        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    {
        //        var str=fs.Read(buffer, 0, buffer.Length);
        //        Debug.Log(str.ToString());
        //        fs.Close();
        //    }
        //}
        //catch (System.UnauthorizedAccessException ex)
        //{
        //    Debug.Log(ex.Message);
        //}

    }

    public void ReadNextLine()
    {
        TextReader input = new StreamReader(path);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 40; i++)
        {
            sb.Append((char)input.Read());
        }
        Debug.Log(sb);


    }
}
