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
                    headers.Add(header.name.ToLower(),header.value);
                }

                Uri uri = new Uri("https://twitter.com/i/api/2/search/adaptive?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_ext_alt_text=true&include_quote_count=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&simple_quoted_tweet=true&q=artflow.ai%2Fimg%3Fi%3D&result_filter=image&count=20&query_source=typed_query&pc=1&spelling_corrections=1&ext=mediaStats%2ChighlightedLabel%2CvoiceInfo");
                string filename = System.IO.Path.GetFileName(uri.LocalPath);

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

                string savePath = basename + Path.DirectorySeparatorChar + filename;

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
    }
}
