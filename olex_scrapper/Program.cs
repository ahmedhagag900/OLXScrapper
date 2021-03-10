using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace olexScrapper
{
    class Program
    {
        static async Task Main(string[] args)
        {   
            //url to scrap from
            var url = "https://www.olx.com.eg/en/electronics-home-appliances/video-games-and-consoles/";
            var _client = new HttpClient();
            var res = await _client.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(res);
            //get the base nodes that contains the data
            var ProductNodes = doc.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("ads ads--list")).ToList();

            List<OlexProductModel> products = new List<OlexProductModel>();
            foreach(var node in ProductNodes) 
            {
                var innernode = node.Descendants("div").Where(y => y.GetAttributeValue("class", "").Equals("ads__item")).ToList();
                foreach (var tag in innernode)
                {
                    //get the product info
                    var item_imgurl = tag.Descendants("img").Where(y => y.GetAttributeValue("class", "")
                    .Contains("ads__item__photos")).FirstOrDefault().GetAttributeValue("src", "");
                    var item_info = tag.Descendants("div").Where(y => y.GetAttributeValue("class", "")
                    .Contains("ads__item__info")).FirstOrDefault();
                    var item_link = item_info.Descendants("a").Where(y => y.GetAttributeValue("class", "")
                    .Contains("ads__item__ad--title")).FirstOrDefault().GetAttributeValue("href", "").Trim();
                    var item_price = item_info.Descendants("p").Where(y => y.GetAttributeValue("class", "")
                    .Contains("ads__item__price price ")).FirstOrDefault().InnerHtml.Trim();
                    var item_name = item_info.Descendants("a").Where(y => y.GetAttributeValue("class", "")
                     .Contains("ads__item__ad--title")).FirstOrDefault().GetAttributeValue("title", "").Trim();

                    //save it into model
                    OlexProductModel model = new OlexProductModel();
                    model.name = item_name;
                    model.image = item_imgurl;
                    model.url = item_link;
                    item_price = Regex.Match(item_price, @"\d+,?\d*.?\d+").Value;
                    double price = 0;
                    double.TryParse(item_price,out price);
                    model.price = price;
                    products.Add(model);
                }
                

            }
            Console.OutputEncoding = System.Text.Encoding.ASCII;
            //display the scrapped data
            products.ForEach((x) => {
                Console.WriteLine("Name: "+x.name );
                Console.WriteLine("Image: " + x.image);
                Console.WriteLine("Link: " + x.url);
                Console.WriteLine("Price: " + x.price+"\n");
            });

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
