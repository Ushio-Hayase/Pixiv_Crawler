﻿using System.Text.Json;
using CommandLine;

namespace pixiv_crawler;

class Program
{
    public class Options {
        [Option('s', Required = true ,HelpText = "keyword to search for")]
        public required string Search {get; set;}

        [Option('p', Default = 5, HelpText = "Number of pages to search")]
        public int Pages {get; set;}

        [Option('t', HelpText = "this option is to select i(illust) or u(ugoira) or a(illust_and_ugoira)")]
        public char Type {get; set;}

        [Option("sort", Default = "new", HelpText= "Sort by to search. this option has two option, new or old")]
        public string Sort {get; set;}

        [Option("mode", Default = "all", HelpText = "Whether to load r18. this option has all, r18, safe")]
        public string Mode {get; set;}

        [Option("output", Default ="./pixiv/" ,HelpText ="storage to store")]
        public string Path {get; set;}

        [Option("filename", Default = "id", HelpText = "filename options can have \"id\" or \"title\"")]
        public string Name {get;set;}
    }

    static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
        .WithParsed(Parsed)
        .WithNotParsed(ParseError);       
             
    }

    static void Parsed(Options options) {
        IllustrationsUrlQuery.Type type;
        IllustrationsUrlQuery.Order order;
        IllustrationsUrlQuery.Mode mode;
        Collector.Name name;

        // Parse Type option
        switch (options.Type) {
            case 'a': 
                type = IllustrationsUrlQuery.Type.illust_and_ugoira;
                break;
            case 'i':
                type = IllustrationsUrlQuery.Type.illust;
                break;
            case 'u':
                type = IllustrationsUrlQuery.Type.ugoira;
                break;
            default:
                Console.Error.WriteLine("Type option parsing error : Type is Not a or i or u");
                return;
        }

        // Parse Sort option
        switch (options.Sort) {
            case "new" :
                order = IllustrationsUrlQuery.Order.date_d;
                break;
            case "old":
                order = IllustrationsUrlQuery.Order.date;
                break;
            default:
                Console.Error.WriteLine("Sort Option Parsing Errror : This Option must be haven new or old");
                return;
        }

        // Parse Mode Option
        switch (options.Mode) {
            case "all" :
                mode = IllustrationsUrlQuery.Mode.all;
                break;
            case "r18":
                mode = IllustrationsUrlQuery.Mode.r18;
                break;
            case "safe":
                mode = IllustrationsUrlQuery.Mode.safe;
                break;
            default:
                Console.Error.WriteLine("Mode Option Parsing Errror : This Option must be haven all or r18 or safe");
                return;
        }

        if(options.Path == "") {
            Console.Error.WriteLine("Output Path Parse Errror : Path is blank");
            return;
        }

        if(!Directory.Exists(options.Path)) {
            Directory.CreateDirectory(options.Path);
        }

        switch (options.Name) {
            case "id" : name = Collector.Name.id; break;
            case "title" : name = Collector.Name.title; break;
            default: Console.Error.WriteLine("unknown option of file name type");return;
        }

        Collector collector = new(options.Search, name);
        Crawler crawler = new();

        List<(string, string)> imglist = collector.Run(options.Pages, type, order, mode).Result;

        List<(string,byte[])> imgs = crawler.GetImageAsync(imglist).Result;

        foreach (var img in imgs) {
            var file = File.Create(Path.Join([options.Path, img.Item1, ".png"]));
            file.Write(img.Item2);
            file.Close();
        }

    }

    static void ParseError(IEnumerable<Error> errs) {
        Console.Error.WriteLine(errs);
    }
}
