using MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text.Json;

namespace HARtoFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                toFiles(arg);
            }
            

            Console.ReadKey();
        }
        static JsonSerializerOptions opt = new JsonSerializerOptions() { NumberHandling= System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };


        public static object MimeMapping { get; private set; }

        static void toFiles(string sourceHARPath)
        {
            string dirname = Path.GetDirectoryName(sourceHARPath);
            string basename=  (dirname == "" ? "" : dirname+ Path.DirectorySeparatorChar)+ Path.GetFileNameWithoutExtension(sourceHARPath);
            Directory.CreateDirectory(basename);

            string content = File.ReadAllText(sourceHARPath);
            Rootobject stuff = JsonSerializer.Deserialize<Rootobject>(content, opt);

            foreach (Entry entry in stuff.log.entries)
            {
                // Headers reading
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach(Header1 header in entry.response.headers)
                {
                    //headers.Add(header.name.ToLower(),header.value);
                    headers[header.name] = header.value;
                }

                Uri uri = new Uri(entry.request.url);
                string filename = uri.Segments.Length == 0 ? "__NONAME__" : MakeValidFileName( uri.Segments[uri.Segments.Length-1].Trim().Trim(new char[] { '\\', '/' }));
                if(filename.Length > 40)
                {
                    filename = filename.Substring(0, 40) + "_TRUNCNAME_";
                }
                string path = MakeValidFileName(basename);
                for(int i = 0; i < uri.Segments.Length - 1; i++)
                {
                    if(uri.Segments[i] == "/" || uri.Segments[i] == "\\")
                    {
                        continue;
                    }
                    string pathseg = MakeValidFileName(uri.Segments[i].Trim().Trim(new char[] { '\\', '/' }));
                    if (pathseg.Length > 40)
                    {
                        pathseg = pathseg.Substring(0, 40) + "_TRUNCDIR_";
                    }
                    path = Path.Combine(path, pathseg);
                }


                if (!filename.Contains("."))
                {
                    string extension = ".raw";

                    if (headers.ContainsKey("content-type"))
                    {
                        try {

                            string[] typeParts = headers["content-type"].Split(';');
                            extension = MimeTypeMap.GetExtension(typeParts[0].Trim());
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Extension from content type failed.");
                        }
                    }

                    filename = filename + extension;
                }
                if (headers.ContainsKey("content-disposition"))
                {
                    try {

                        ContentDisposition contentDisposition = new ContentDisposition(headers["content-disposition"]);
                        if(contentDisposition.FileName != null)
                        {
                            filename = contentDisposition.FileName;
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Extension from content type failed.");
                    }
                }

                string savePath = Path.Combine(path, filename);

                Directory.CreateDirectory(path);

                savePath = GetUnusedFilename(savePath);



                if(entry.response.content.encoding != null && entry.response.content.encoding.ToLower() == "base64")
                {

                    File.WriteAllBytes(savePath, Convert.FromBase64String(entry.response.content.text));
                } else
                {

                    File.WriteAllText(savePath, entry.response.content.text);
                }

                Console.WriteLine(savePath + " written.");

            }
        }

        public static string GetUnusedFilename(string baseFilename)
        {
            if (!File.Exists(baseFilename))
            {
                return baseFilename;
            }
            string extension = Path.GetExtension(baseFilename);

            int index = 1;
            while (File.Exists(Path.ChangeExtension(baseFilename, "." + (++index) + extension))) ;

            return Path.ChangeExtension(baseFilename, "." + (index) + extension);
        }




        // from: https://stackoverflow.com/a/847251
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
