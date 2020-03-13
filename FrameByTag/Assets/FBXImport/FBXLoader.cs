using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text.RegularExpressions;

public class FBXLoader
{
    /// <summary>
    /// Load an FBX file from a file path. MTL?
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject Load(string path)
    {
        if (File.Exists(path))
        {

        }
        else if (File.Exists(path + ".fbx"))
        {
            path += ".fbx";
        }
        
        var source = new FileInfo(path);
        var sr = new StreamReader(path);

        //file reading setup 
        sr.BaseStream.Position = 0;
        sr.DiscardBufferedData();
        //file read
        var fileStr = sr.ReadToEnd();
        sr.Close();

        int currentCh = 0;
        //file parse
        //while (currentCh < fileStr.Length)
        //{

        //    if (currentCh >= fileStr.Length)
        //    {
        //        return null;
        //    }

        //}

        #region pasted_from_git
        /*
        while (iterator < fileCompl.Length)
        {
            ReadSkip(fileCompl, white, ref iterator); //puts the iterator on the start of the next word
            currentWord = ReadTill(fileCompl, white, ref iterator);

            if (iterator >= fileCompl.Length)
            {
                FinalizeAsset();
                return;
            } 


            if (currentWord[0] == ';')
            {
                //comment line
                //goes to the next line and restarts the loop
                Utilities.ReadTill(fileCompl, newLn, ref iterator);
                Debug.Log(@fileCompl[iterator]);
                continue;
            } // if

            if (currentWord[0] == '\"')
            {
                //handle quoted information. For now this just include the model name
                //in syntax "Model::NAME"
                continue;
            } // if

            if (currentWord[0] == '{')
            {
                depth++;
                continue;
            } // if

            if (currentWord[0] == '}')
            {
                depth--;
                continue;
            } // if

            switch (currentWord.ToString())
            {
                case "FBXHeaderExtension:":
                    SkipSection();
                    break;
                case "GlobalSettings:":
                    SkipSection();
                    break;
                case "Documents:":
                    SkipSection();
                    break;
                case "References:":
                    SkipSection();
                    break;
                case "Definitions:":
                    SkipSection();
                    break;
                case "Takes:":
                    SkipSection();
                    break;

                case "Objects:":
                    ObjectBuilding();
                    break;

                case "Connections:":
                    Connections();
                    break;

                default:
                    break;
            } //switch
        } //while(true)
        */
        #endregion


        var freader = new FBXWordReader(path);
        //freader.ReadAllLines();
        //freader.ReadLine();
        freader.ReadNextLine();


        //return new GameObject();
        //return Load(path)
        return new GameObject();

    }

    public void Import(string filename)
    {
        /*
         
        using (var reader = new StreamReader(filename))
        {
            var parser = new Parser(new Tokenizer(reader, filename: filename));
            var builder = new FBXObjectBuilder();

            var pobjects = parser.ReadFile();
            var loadedSceneObject = converter.ConvertScene(pobjects);

            return loadedSceneObject;
        }
        */
    }
    public static string ReadSkip(string file, char[] skip, ref int start)
    {
        int startHold;
        startHold = start;

        for (; start < file.Length; start++)
        {
            if (file[start] == '\0')
            {
                string result = file.Substring(startHold, start - startHold + 1);
                return result;
            } 

            for (int i = 0; i < skip.Length; i++)
            {
                if (file[start] == skip[i])
                {
                    break;
                }
                else if (file[start] != skip[i] && i < skip.Length - 1)
                {
                    continue;
                } 
                string result = file.Substring(startHold, start - startHold);
                return result;

            } // for
        } // for
        return new string(new char[] { 'f', 'a', 'i', 'l', ' ', 's', 'k', 'i', 'p' }); //required to compile
    }
    public static string ReadTill(string file, char[] until, ref int start)
    {
        int startHold;
        startHold = start;

        for (; start < file.Length; start++)
        {
            if (file[start] == '\0')
            {
                string ret = file.Substring(startHold, start - startHold + 1);
                return ret;
            }

            for (int i = 0; i < until.Length; i++)
            {
                if (file[start] == until[i])
                {
                    string ret = file.Substring(startHold, start - startHold);
                    return ret;
                }
            }
        }

        //not the best way to handle EOF
        if (start == file.Length)
        {
            string ret = file.Substring(startHold, start - startHold);
            return ret;
        }

        return new string(new char[] { 'f', 'a', 'i', 'l', ' ', 't', 'i', 'l', 'l' });
    }
}

        
