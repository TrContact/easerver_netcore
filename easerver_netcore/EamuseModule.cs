using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Nancy;

namespace easerver_netcore {
    // ReSharper disable once UnusedMember.Global
    public class EamuseModule : NancyModule {
        static Dictionary<int, string> easess = new Dictionary<int, string>();

        static readonly byte[] Key =
            "00000000000069D74627D985EE2187161570D08D93B12455035B6DF0D8205DF5".HexToBytes();

        public EamuseModule() {
            Get("/", args => {
                //return Encoding.GetEncoding(Utils.m_encoding).GetString(sdvx_3s2_game_common());
                // ReSharper disable once ConvertToLambdaExpression
                return "WebUI not available";
            });

            Get("/{_*}", args => "WebUI not available");

            Post("/{_*}", args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] UNHANDLED: {1}", DateTime.Now, Request.Url);
                return "Not supported game";
            });

            #region slash 0

            Post("/service/services/services",  args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] services: {1}", DateTime.Now, Request.Url);
                DynamicDictionary param = parse_request_param(Request.Query);
                byte[] data = services_get(param["model"]);
                return generate_response(data);
            });

            Post("/service/services/cardmng", args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] cardmng: {1}", DateTime.Now, Request.Url);
                DynamicDictionary param = parse_request_param(Request.Query);
                byte[] data = cardmng_dispatch(param["method"]);
                return generate_response(data);
            });

            Post("/service/services/eacoin",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] eacoin: {1}", DateTime.Now, Request.Url);
                DynamicDictionary param = parse_request_param(Request.Query);
                byte[] data = eacoin_dispatch(param["method"]);
                return generate_response(data);
            });

            #region static

            Post("/service/services/message",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] message: {1}", DateTime.Now, Request.Url);
                byte[] data = message_get();
                return generate_response(data);
            });

            Post("/service/services/package", args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] package: {1}", DateTime.Now, Request.Url);
                byte[] data = package_list();
                return generate_response(data);
            });

            Post("/service/services/pcbtracker",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] pcbtracker: {1}", DateTime.Now, Request.Url);
                byte[] data = pcbtracker_alive();
                return generate_response(data);
            });

            Post("/service/services/pcbevent",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] pcbevent: {1}", DateTime.Now, Request.Url);
                byte[] data = pcbevent_put(Request.Query["model"]);
                return generate_response(data);
            });

            Post("/service/services/facility",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] facility: {1}", DateTime.Now, Request.Url);
                byte[] data = facility_get();
                return generate_response(data);
            });

            #endregion

            Post("/service/services/sdvx",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] sdvx: {1}", DateTime.Now, Request.Url);
                DynamicDictionary param = parse_request_param(Request.Query);
                byte[] data = sdvx_dispatch(param["model"], param["module"], param["method"]);
                return generate_response(data);
            });

            Post("/service/services/bst",args => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] bst: {1}", DateTime.Now, Request.Url);
                DynamicDictionary param = parse_request_param(Request.Query);
                byte[] data = bst_dispatch(param["model"], param["module"], param["method"]);
                return generate_response(data);
            });

            #endregion

            #region slash 1

            Post("/services/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] services: {1}", DateTime.Now, Request.Url);
                string model = param.model;
                byte[] data = services_get(model);
                return generate_response(data);
            });

            Post("/service/services/services/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] services: {1}", DateTime.Now, Request.Url);
                string model = param.model;
                byte[] data = services_get(model);
                return generate_response(data);
            });

            Post("/service/services/cardmng/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] cardmng: {1}", DateTime.Now, Request.Url);
                string method = param.method;
                byte[] data = cardmng_dispatch(method);
                return generate_response(data);
            });

            Post("/service/services/eacoin/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] eacoin: {1}", DateTime.Now, Request.Url);
                string method = param.method;
                byte[] data = eacoin_dispatch(method);
                return generate_response(data);
            });

            #region static

            Post("/service/services/message/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] message: {1}", DateTime.Now, Request.Url);
                byte[] data = message_get();
                return generate_response(data);
            });

            Post("/service/services/package/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] package: {1}", DateTime.Now, Request.Url);
                byte[] data = package_list();
                return generate_response(data);
            });

            Post("/service/services/pcbtracker/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] pcbtracker: {1}", DateTime.Now, Request.Url);
                byte[] data = pcbtracker_alive();
                return generate_response(data);
            });

            Post("/service/services/pcbevent/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] pcbevent: {1}", DateTime.Now, Request.Url);
                byte[] data = pcbevent_put(param.model);
                return generate_response(data);
            });

            Post("/service/services/facility/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] facility: {1}", DateTime.Now, Request.Url);
                byte[] data = facility_get();
                return generate_response(data);
            });

            #endregion

            Post("/service/services/sdvx/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] sdvx: {1}", DateTime.Now, Request.Url);
                string model = param.model;
                string module = param.module;
                string method = param.method;
                byte[] data = sdvx_dispatch(model, module, method);
                return generate_response(data);
            });

            Post("/service/services/iidx/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] iidx: {1}", DateTime.Now, Request.Url);
                string model = param.model;
                string module = param.module;
                string method = param.method;
                byte[] data = iidx_dispatch(model, module, method);
                return generate_response(data);
            });

            Post("/service/services/bst/{model}/{module}/{method}", param => {
                Console.WriteLine("[{0:yyyy/MM/dd HH:mm:ss}] iidx: {1}", DateTime.Now, Request.Url);
                string model = param.model;
                string module = param.module;
                string method = param.method;
                byte[] data = bst_dispatch(model, module, method);
                return generate_response(data);
            });

            #endregion
        }

        Response generate_response(byte[] input) {
            //            byte[] part = new byte[6];
            //            Random rand = new Random();
            //            rand.NextBytes(part);
            byte[] output = null;
            Utils.XMLToBinary(input, ref output);
            bool compress = Request.Headers["X-Compress"].FirstOrDefault() == "lz77";
            bool crypt = Request.Headers["X-Eamuse-Info"].FirstOrDefault() != null;
            if (compress) {
                output = Utils.CompressEmpty(output);
            }
            if (crypt) {
                string[] orig = Request.Headers["X-Eamuse-Info"].FirstOrDefault()?.Split('-');
                byte[] part = (orig[1] + orig[2]).HexToBytes();
                for (int i = 0; i < 6; i++)
                    Key[i] = part[i];
                output = Utils.RC4.Encrypt(new MD5CryptoServiceProvider().ComputeHash(Key), output);
            }
            MemoryStream stream = new MemoryStream(output);
            //            MemoryStream stream = new MemoryStream(input);
            Response rsp = Response.FromStream(stream, "application/octet-stream");
            if (crypt) {
                string header = Request.Headers["X-Eamuse-Info"].FirstOrDefault();
                rsp = rsp.WithHeader("X-Eamuse-Info", header);
            }
            if (compress) {
                rsp = rsp.WithHeader("X-Compress", "lz77");
            } else {
                rsp = rsp.WithHeader("X-Compress", "none");
            }
            return rsp;
        }

        XDocument parse_request() {
            int index = 0;
            string info = Request.Headers["X-Eamuse-Info"].FirstOrDefault();
            byte[] data = new byte[Request.Headers.ContentLength];
            Request.Body.Read(data, 0, (int) Request.Headers.ContentLength);
            if (info != null) {
                string[] str_array2 = info.Split('-');

                do {
                    Key[index] = Convert.ToByte((str_array2[1] + str_array2[2]).Substring(index << 1, 2), 0x10);
                    index++;
                } while (index < 6);
                byte[] rc4_key = new MD5CryptoServiceProvider().ComputeHash(Key);
                data = Utils.RC4.Decrypt(rc4_key, data);
            }
            string header2 = Request.Headers["X-Compress"].FirstOrDefault();
            if (header2 == "lz77")
                data = Utils.Decompress(data);
            byte[] buffer = null;
            Utils.BinaryToXML(data, ref buffer);
            return XDocument.Parse(Encoding.GetEncoding(932).GetString(buffer));
            //            return XDocument.Parse(Encoding.GetEncoding(932).GetString(data));
        }

        DynamicDictionary parse_request_param(DynamicDictionary param) {
            DynamicDictionary ret = new DynamicDictionary();
            if (param.ContainsKey("model")) {
                ret["model"] = param["model"];
            }
            if (param.ContainsKey("module")) {
                ret["module"] = param["module"];
            }
            if (param.ContainsKey("method")) {
                ret["method"] = param["method"];
            }
            if (param.ContainsKey("f")) {
                string f = param["f"];
                ret["module"] = f.Split('.')[0];
                ret["method"] = f.Split('.')[1];
            }
            return ret;
        }

        #region dispatch

        byte[] cardmng_dispatch(string method) {
            switch (method) {
            case "inquire":
                return cardmng_inquire();
            case "getrefid":
                return cardmng_getrefid();
            case "authpass":
                return cardmng_authpass();
            case "bindmodel":
                return cardmng_bindmodel();
            default:
                return new byte[0];
            }
        }

        byte[] eacoin_dispatch(string method) {
            switch (method) {
            case "opcheckin":
                return eacoin_opcheckin();
            case "getlog":
                return eacoin_getlog();
            case "opcheckout":
                return eacoin_checkout();
            case "checkin":
                return eacoin_checkin();
            case "checkout":
                return eacoin_checkout();
            case "consume":
                return eacoin_consume();
            default:
                return new byte[0];
            }
        }

        byte[] sdvx_dispatch(string model, string module, string method) {
            if (module == "eventlog") {
                return eventlog_write();
            }
            switch (model.Split(':')[4]) {
            case "2016121200": // 3S2 final
                return sdvx_3s2_dispatch(module, method);
            default:
                return new byte[0];
            }
        }

        byte[] sdvx_3s2_dispatch(string module, string method) {
            switch (module) {
            case "game_3":
                return sdvx_3s2_game_dispatch(method);
            default:
                return new byte[0];
            }
        }

        byte[] sdvx_3s2_game_dispatch(string method) {
            switch (method) {
            case "common":
                return sdvx_3s2_game_common();
            case "hiscore":
                return sdvx_3s2_game_hiscore();
            case "shop":
                return sdvx_3s2_game_shop();
            case "exception":
                return sdvx_3s2_game_exception();
            case "load":
                return sdvx_3s2_game_load();
            case "load_m":
                return sdvx_3s2_game_loadm();
            case "load_r":
                return sdvx_3s2_game_loadr();
            case "frozen":
                return sdvx_3s2_game_frozen();
            case "new":
                return sdvx_3s2_game_new();
            case "lounge":
                return sdvx_3s2_game_lounge();
            case "entry_s":
                return sdvx_3s2_game_entry_s();
            case "entry_e":
                return sdvx_3s2_game_entry_e();
            case "save_m":
                return sdvx_3s2_game_savem();
            case "save_e":
                return sdvx_3s2_game_savee();
            case "save_c":
                return sdvx_3s2_game_savec();
            case "buy":
                return sdvx_3s2_game_buy();
            case "save":
                return sdvx_3s2_game_save();
            case "play_e":
                return sdvx_3s2_game_playe();
            case "print":
                return sdvx_3s2_game_print();
            case "print_h":
                return sdvx_3s2_game_printh();
            default:
                return new byte[0];
            }
        }

        byte[] iidx_dispatch(string model, string module, string method) {
            switch (model.Split(':')[4]) {
            case "2017150500": // 24 leak
                return iidx24_dispatch(module, method);
            default:
                return new byte[0];
            }
        }

        byte[] iidx24_dispatch(string module, string method) {
            switch (module) {
            case "IIDX24pc":
                return iidx24_pc_dispatch(method);
            //            case "IIDX24music":
            //                return sdvx_3s2_game_dispatch(method);
            //            case "IIDX24grade":
            //                return sdvx_3s2_game_dispatch(method);
            case "IIDX24shop":
                return iidx24_shop_dispatch(method);
            //            case "IIDX24ranking":
            //                return sdvx_3s2_game_dispatch(method);
            default:
                return new byte[0];
            }
        }

        byte[] iidx24_shop_dispatch(string method) {
            switch (method) {
            case "getname":
                return iidx24_shop_getname();
            default:
                return new byte[0];
            }
        }

        byte[] iidx24_pc_dispatch(string method) {
            switch (method) {
            case "common":
                return iidx24_pc_common();
            default:
                return new byte[0];
            }
        }

        byte[] bst_dispatch(string model, string module, string method) {
            //if (module == "eventlog") {
            //    return eventlog_write();
            //}
            switch (model.Split(':')[4]) {
            case "2016111400": // 2
                return bst_2_dispatch(module, method);
            default:
                return new byte[0];
            }
        }

        byte[] bst_2_dispatch(string module, string method) {
            switch (module) {
            case "pcb2":
                return bst_2_pcb2_dispatch(method);
            case "shop2":
                return bst_2_shop2_dispatch(method);
            case "info2":
                return bst_2_info2_dispatch(method);
            case "player2":
                return bst_2_player2_dispatch(method);
            default:
                return new byte[0];
            }
        }

        byte[] bst_2_pcb2_dispatch(string method) {
            switch (method) {
            case "error":
                return bst_2_pcb2_error();
            case "boot":
                return bst_2_pcb2_boot();
            case "uptime_update":
                return bst_2_pcb2_uptime_update();
            default:
                return new byte[0];
            }
        }

        byte[] bst_2_shop2_dispatch(string method) {
            switch (method) {
            case "setting_write":
                return bst_2_shop2_setting_write();
            default:
                return new byte[0];
            }
        }

        byte[] bst_2_info2_dispatch(string method) {
            switch (method) {
            case "common":
                return bst_2_info2_common();
                case "music_count_read":
                    return bst_2_info2_music_count_read();
                case "music_ranking_read":
                    return bst_2_info2_music_ranking_read();
            default:
                return new byte[0];
            }
        }

        byte[] bst_2_player2_dispatch(string method) {
            switch (method) {
            case "start":
            case "continue":
                return bst_2_player2_start();
            case "succeed":
                return bst_2_player2_succeed();
            case "write":
                return bst_2_player2_write();
            case "stagedata_write":
                return bst_2_player2_stagedata_write();
            case "course_stage_data_write":
                return bst_2_player2_course_stage_data_write();
            case "course_data_write":
                return bst_2_player2_course_data_write();
            case "read":
                return bst_2_player2_read();
            case "end":
                return bst_2_player2_end();
            default:
                return new byte[0];
            }
        }

        #endregion

        #region static

        static byte[] pcbtracker_alive() {
            // we will try to enable paseli
            // we dont have pcbid verify right now
            XDocument doc = new XDocument(new XElement("response",
                new XElement("pcbtracker", new XAttribute("ecenable", "1"))));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] facility_get() {
            // play with this!
            XElement facility = new XElement("facility");
            XElement location = new XElement("location",
                new XElement("id", "00000000"),
                new XElement("country", "NA"),
                new XElement("region", "NA"),
                new XElement("name", "アーケード（仮）"),
                new XElement("type", 0, new XAttribute("__type", "u8"))
            );
            XElement line = new XElement("line",
                new XElement("id", ""),
                new XElement("class", 0, new XAttribute("__type", "u8"))
            );
            XElement portfw = new XElement("portfw",
                new XElement("globalip", "1.0.0.127", new XAttribute("__type", "ip4")), // is this correct?
                new XElement("globalport", 1234, new XAttribute("__type", "u16")), // hardcode atm
                new XElement("privateport", 1234, new XAttribute("__type", "u16"))
            );
            XElement _public = new XElement("public",
                new XElement("flag", 1, new XAttribute("__type", "u8")),
                new XElement("name", "アーケード（仮）"),
                new XElement("latitude", "0.0"),
                new XElement("longitude", "0.0")
            );
            XElement share = new XElement("share",
                new XElement("eacoin",
                    new XElement("notchamount", 0, new XAttribute("__type", "s32")),
                    new XElement("notchcount", 0, new XAttribute("__type", "s32")),
                    new XElement("supplylimit", 100000, new XAttribute("__type", "s32"))),
                new XElement("url",
                    new XElement("eapass", "http://eagate.573.jp"),
                    new XElement("arcadefan", "http://eagate.573.jp"),
                    new XElement("konaminetdx", "http://eagate.573.jp"),
                    new XElement("konamiid", "http://eagate.573.jp"),
                    new XElement("eagate", "http://eagate.573.jp"))
            );

            facility.Add(location, line, portfw, _public, share);
            XDocument doc = new XDocument(new XElement("response", facility));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] pcbevent_put(string model) {
            // we are not recording this now
            XDocument doc = new XDocument(new XElement("response", new XElement(model.Split(':')[0])));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] eventlog_write() {
            XElement eventlog = new XElement("eventlog",
                new XElement("gamesession", 1, new XAttribute("__type", "s64")),
                new XElement("logsendflg", 0, new XAttribute("__type", "s32")),
                new XElement("logerrlevel", 0, new XAttribute("__type", "s32")),
                new XElement("evtidnosendflg", 0, new XAttribute("__type", "s32"))
            );
            XDocument doc = new XDocument(new XElement("response", eventlog));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] message_get() {
            // unknown, copy from a
            XDocument doc = new XDocument(new XElement("response", new XElement("message")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] package_list() {
            // live update?
            XDocument doc = new XDocument(new XElement("response", new XElement("package")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion

        static byte[] services_get(string model) {
            // /call(model=version, srcid=pcbid, tag=???)/services(method=get)/info/avs2=version
            // we dont care avs version, hardcode output
            const string commonurl = "http://eamuse.konami.fun/service/services/";
            XElement services = new XElement("services", new XAttribute("expire", "3600"),
                new XAttribute("method", "get"), new XAttribute("mode", "operation"), new XAttribute("status", "0"));
            string[] core = {
                "cardmng", "facility", "message", "numbering", "package", "pcbevent", "pcbtracker", "pkglist",
                "posevent", "userdata", "userid", "eacoin", "dlstatus", "netlog", "info", "reference", "sidmgr"
            };
            foreach (string item in core)
                services.Add(
                    new XElement("item", new XAttribute("name", item), new XAttribute("url", commonurl + item)));
            services.Add(new XElement("item", new XAttribute("name", "ntp"),
                new XAttribute("url", "ntp://eamuse.konami.fun/")));
            services.Add(new XElement("item", new XAttribute("name", "keepalive"),
                new XAttribute("url",
                    "http://eamuse.konami.fun/keepalive?pa=127.0.0.1&ia=127.0.0.1&ga=127.0.0.1&ma=127.0.0.1&t1=2&t2=10")));
            switch (model.Split(':')[0]) {
            case "KFC":
                const string sdvxurl = "http://r.mrx.im/sdvx";
                string[] kfc = {
                    "local", "local2", "lobby", "slocal", "slocal2", "sglocal", "sglocal2", "lab", "globby",
                    "slobby", "sglobby"
                };
                foreach (string item in kfc)
                    services.Add(new XElement("item", new XAttribute("name", item), new XAttribute("url", sdvxurl)));
                break;
            case "LDJ":
                const string iidxurl = "http://127.0.0.1/iidx";
                string[] ldj = {
                    "local", "local2", "lobby", "slocal", "slocal2", "sglocal", "sglocal2", "lab", "globby",
                    "slobby", "sglobby"
                };
                foreach (string item in ldj)
                    services.Add(new XElement("item", new XAttribute("name", item), new XAttribute("url", iidxurl)));
                break;
            case "NBT":
                const string bsturl = "http://eamuse.konami.fun/service/services/bst";
                string[] bst = {
                    "local"
                };
                foreach (string item in bst)
                    services.Add(new XElement("item", new XAttribute("name", item), new XAttribute("url", bsturl)));
                break;
            }

            XDocument doc = new XDocument(new XElement("response", services));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #region card management

        byte[] cardmng_inquire() {
            XDocument req = parse_request();
            XElement mng = req.XPathSelectElement("/call/cardmng");
            string cardid = mng.Attribute("cardid")?.Value;
            string dataid;
            // offline part, disable card
            //XElement cardmng = new XElement("cardmng");
            //cardmng.Add(new XAttribute("status", -1));
            //XDocument doc = new XDocument(new XElement("response", cardmng));
            //return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            if (DB.card_inquire(cardid, out dataid)) {
                XElement cardmng = new XElement("cardmng");
                string model = Request.Query["model"];
                cardmng.Add(DB.card_binded(model, dataid) ? new XAttribute("binded", 1) : new XAttribute("binded", 0));
                cardmng.Add(new XAttribute("dataid", dataid));
                cardmng.Add(new XAttribute("ecflag", 1));
                cardmng.Add(new XAttribute("expired", 0));
                cardmng.Add(new XAttribute("newflag", 0));
                cardmng.Add(new XAttribute("refid", dataid));
                XDocument doc = new XDocument(new XElement("response", cardmng));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            } else {
                XElement cardmng = new XElement("cardmng");
                cardmng.Add(new XAttribute("status", 112));
                XDocument doc = new XDocument(new XElement("response", cardmng));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
        }

        byte[] cardmng_getrefid() {
            XDocument req = parse_request();
            XElement mng = req.XPathSelectElement("/call/cardmng");
            string passwd = mng.Attribute("passwd")?.Value;
            string cardid = mng.Attribute("cardid")?.Value;
            byte[] r = new byte[8];
            Random rand = new Random();
            rand.NextBytes(r);
            string dataid = r.ToHex().ToUpper();
            DB.card_create(cardid, dataid, passwd);
            XElement cardmng = new XElement("cardmng");
            cardmng.Add(new XAttribute("dataid", dataid));
            cardmng.Add(new XAttribute("refid", dataid));
            XDocument doc = new XDocument(new XElement("response", cardmng));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] cardmng_authpass() {
            XDocument req = parse_request();
            XElement mng = req.XPathSelectElement("/call/cardmng");
            string passwd = mng.Attribute("pass")?.Value;
            string refid = mng.Attribute("refid")?.Value;
            if (DB.card_auth(refid, passwd)) {
                XElement cardmng = new XElement("cardmng");
                cardmng.Add(new XAttribute("status", 0));
                XDocument doc = new XDocument(new XElement("response", cardmng));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            } else {
                XElement cardmng = new XElement("cardmng");
                cardmng.Add(new XAttribute("status", 116));
                XDocument doc = new XDocument(new XElement("response", cardmng));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
        }

        byte[] cardmng_bindmodel() {
            XDocument req = parse_request();
            XElement mng = req.XPathSelectElement("/call/cardmng");
            string refid = mng.Attribute("refid")?.Value;
            string model = Request.Query["model"];
            DB.card_bind(model, refid);
            XElement cardmng = new XElement("cardmng");
            XDocument doc = new XDocument(new XElement("response", cardmng));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion

        #region eacoin

        byte[] eacoin_opcheckin() {
            XDocument req = parse_request();
            string passwd = req.XPathSelectElement("/call/eacoin/passwd").Value;
            if (passwd == "11451419") {
                Random rand = new Random();
                XElement eacoin = new XElement("eacoin");
                int sessid = rand.Next(0, 1000);
                eacoin.Add(new XElement("sessid", sessid));
                easess[sessid] = "0";
                XDocument doc = new XDocument(new XElement("response", eacoin));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            } else {
                XElement eacoin = new XElement("eacoin", new XAttribute("status", 116));
                // 116, 15, 12: passwd error
                // 128: cannot use paseli
                eacoin.Add(new XElement("sessid", "0"));
                XDocument doc = new XDocument(new XElement("response", eacoin));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
        }

        byte[] eacoin_getlog() {
            XDocument req = parse_request();
            string logtype = req.XPathSelectElement("/call/eacoin/logtype").Value;
            XElement eacoin = new XElement("eacoin");
            XDocument doc = new XDocument(new XElement("response", eacoin));
            XElement topic = new XElement("topic");
            int perpage;
            XElement summary;
            XElement items;
            switch (logtype) {
            case "last7days":
                topic.Add(new XElement("sumdate", "11"));
                topic.Add(new XElement("sumfrom", "placeholder"));
                topic.Add(new XElement("sumto", "data"));
                topic.Add(new XElement("today", 11, new XAttribute("__type", "s32")));
                topic.Add(new XElement("average", 12, new XAttribute("__type", "s32")));
                topic.Add(new XElement("total", 13, new XAttribute("__type", "s32")));
                summary = new XElement("summary");
                items = new XElement("items", "1 2 3 4 5 6 7", new XAttribute("__type", "s32"),
                    new XAttribute("__count", 7));
                summary.Add(items);
                eacoin.Add(new XElement("last7days", topic, summary));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            case "last52weeks":
                topic.Add(new XElement("sumdate", "21"));
                topic.Add(new XElement("sumfrom", "not available"));
                topic.Add(new XElement("sumto", "now"));
                summary = new XElement("summary");
                items = new XElement("items",
                    "1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2",
                    new XAttribute("__type", "s32"), new XAttribute("__count", 52));
                summary.Add(items);
                eacoin.Add(new XElement("last52weeks", topic, summary));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            case "eachday":
                topic.Add(new XElement("sumdate", "31"));
                topic.Add(new XElement("sumfrom", "log is ready"));
                topic.Add(new XElement("sumto", "not presented"));
                summary = new XElement("summary");
                items = new XElement("items", "1 2 3 4 5 6 7", new XAttribute("__type", "s32"),
                    new XAttribute("__count", 7));
                summary.Add(items);
                eacoin.Add(new XElement("eachday", topic, summary));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            case "eachhour":
                topic.Add(new XElement("sumdate", "41"));
                topic.Add(new XElement("sumfrom", "available"));
                topic.Add(new XElement("sumto", "soon"));
                summary = new XElement("summary");
                items = new XElement("items", "1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4",
                    new XAttribute("__type", "s32"), new XAttribute("__count", 24));
                summary.Add(items);
                eacoin.Add(new XElement("eachhour", topic, summary));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            case "detail":
                perpage = int.Parse(req.XPathSelectElement("/call/eacoin/perpage").Value);
                int page = int.Parse(req.XPathSelectElement("/call/eacoin/page").Value);
                topic.Add(new XElement("sumdate", "PASELI!"));
                topic.Add(new XElement("sumfrom", "52"));
                topic.Add(new XElement("sumto", "53"));
                XElement history = new XElement("history");
                for (int i = 0; i < perpage; i++) {
                    XElement item = new XElement("item");
                    item.Add(new XElement("date", "2010-01-01 01:01:01",
                        new XAttribute("__type", "str")));
                    item.Add(new XElement("consume", i,
                        new XAttribute("__type", "s32")));
                    item.Add(new XElement("service", 0,
                        new XAttribute("__type", "s32")));
                    item.Add(new XElement("cardtype", "1",
                        new XAttribute("__type", "str")));
                    item.Add(new XElement("cardno", "2JPTPJYU82GHHG1D",
                        new XAttribute("__type", "str")));
                    item.Add(new XElement("title", "Title",
                        new XAttribute("__type", "str")));
                    item.Add(new XElement("systemid", "1099",
                        new XAttribute("__type", "str")));
                    history.Add(item);
                }

                eacoin.Add(new XElement("detail", topic, history));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            case "lastmonths":
                perpage = int.Parse(req.XPathSelectElement("/call/eacoin/perpage").Value);
                topic.Add(new XElement("sumdate", "61"));
                topic.Add(new XElement("sumfrom", "62"));
                topic.Add(new XElement("sumto", "63"));
                summary = new XElement("summary");
                eacoin.Add(new XElement("lastmonths", topic, summary));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            default:
                eacoin.Add(new XElement("sessid", "1"));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
        }

        byte[] eacoin_checkout() {
            XDocument req = parse_request();
            int sessid = Convert.ToInt32(req.XPathSelectElement("/call/eacoin/sessid").Value);
            easess.Remove(sessid);
            XElement eacoin = new XElement("eacoin");
            XDocument doc = new XDocument(new XElement("response", eacoin));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] eacoin_checkin() {
            XDocument req = parse_request();
            string cardid = req.XPathSelectElement("/call/eacoin/cardid").Value;
            XElement eacoin = new XElement("eacoin", new XAttribute("status", 0));
            // 10 11 15 16 23, 17, 128: cannot use paseli
            // 116 passwd error
            eacoin.Add(new XElement("sequence", 1, new XAttribute("__type", "s16")));
            eacoin.Add(new XElement("acstatus", 0, new XAttribute("__type", "u8")));
            eacoin.Add(new XElement("acid", "acid"));
            eacoin.Add(new XElement("acname", "acname"));
            eacoin.Add(new XElement("balance", DB.eacoin_balance(cardid), new XAttribute("__type", "s32")));
            Random rand = new Random();
            int sessid = rand.Next(1000, 10000000);
            eacoin.Add(new XElement("sessid", sessid));
            easess[sessid] = cardid;
            // use sessid to save checkin info
            // used to distinguish different getlog requests
            // game will call getlog.detail
            XDocument doc = new XDocument(new XElement("response", eacoin));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] eacoin_consume() {
            XDocument req = parse_request();
            int sessid = Convert.ToInt32(req.XPathSelectElement("/call/eacoin/sessid").Value);
            int payment = Convert.ToInt32(req.XPathSelectElement("/call/eacoin/payment").Value);
            int service = Convert.ToInt32(req.XPathSelectElement("/call/eacoin/service").Value);
            XElement eacoin = new XElement("eacoin", new XAttribute("status", 0));
            eacoin.Add(new XElement("acstatus", 0, new XAttribute("__type", "u8")));
            eacoin.Add(new XElement("autocharge", 0, new XAttribute("__type", "u8")));
            eacoin.Add(new XElement("balance", DB.eacoin_consume(easess[sessid], payment, service),
                new XAttribute("__type", "s32")));
            XDocument doc = new XDocument(new XElement("response", eacoin));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion

        #region Sound Voltex III Gravity Wars Season2

        static byte[] sdvx_3s2_game_common() {
            XElement e = new XElement("event");
            for (int i = 0; i < 128; i++) {
                if (i == 79) // 66
                    continue;
                e.Add(new XElement("info", new XElement("event_id", i, new XAttribute("__type", "u32"))));
            }
            XElement music_limited = new XElement("music_limited");
            XElement skill_course = new XElement("skill_course");
            //            foreach (IDataRecord item in DB.sdvx_3s2_get_music_list()) {
            //                XElement info = new XElement("info");
            //                info.Add(new XElement("music_id", item["music_id"], new XAttribute("__type", "s32")));
            //                info.Add(new XElement("music_type", item["difficulty"], new XAttribute("__type", "u8")));
            //                info.Add(new XElement("limited", 3, new XAttribute("__type", "u8")));
            //                music_limited.Add(info);
            //            }
            foreach (IDataRecord item in DB.sdvx_3s2_get_skill_courses()) {
                XElement info = new XElement("info");
                info.Add(new XElement("course_id", item["course_id"], new XAttribute("__type", "s16")));
                info.Add(new XElement("level", item["level"], new XAttribute("__type", "s16")));
                info.Add(new XElement("season_id", item["season_id"], new XAttribute("__type", "s32")));
                info.Add(new XElement("season_name", item["season_name"]));
                info.Add(new XElement("season_new_flg", item["season_new_flg"],
                    new XAttribute("__type", "bool")));
                info.Add(new XElement("course_name", item["course_name"]));
                info.Add(new XElement("course_type", item["course_type"], new XAttribute("__type", "s16")));
                info.Add(new XElement("skill_name_id", item["skill_name_id"], new XAttribute("__type", "s16")));
                info.Add(new XElement("matching_assist", item["matching_assist"],
                    new XAttribute("__type", "bool")));
                info.Add(new XElement("gauge_type", item["gauge_type"], new XAttribute("__type", "s16")));
                info.Add(new XElement("paseli_type", item["paseli_type"], new XAttribute("__type", "s16")));
                info.Add(new XElement("track", new XElement("track_no", 0, new XAttribute("__type", "s16")),
                    new XElement("music_id", item["track0_id"], new XAttribute("__type", "s32")),
                    new XElement("music_type", item["track0_type"], new XAttribute("__type", "s8"))));
                info.Add(new XElement("track", new XElement("track_no", 1, new XAttribute("__type", "s16")),
                    new XElement("music_id", item["track1_id"], new XAttribute("__type", "s32")),
                    new XElement("music_type", item["track1_type"], new XAttribute("__type", "s8"))));
                info.Add(new XElement("track", new XElement("track_no", 2, new XAttribute("__type", "s16")),
                    new XElement("music_id", item["track2_id"], new XAttribute("__type", "s32")),
                    new XElement("music_type", item["track2_type"], new XAttribute("__type", "s8"))));
                skill_course.Add(info);
            }
            XDocument doc = new XDocument(new XElement("response",
                new XElement("game_3", e, music_limited, skill_course)));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_hiscore() {
            XElement hit = new XElement("hit");
            XElement sc = new XElement("sc");
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3", hit, sc)));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_shop() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_exception() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_load() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/game_3/refid").Value;
            XDocument doc;
            XElement game;
            foreach (IDataRecord item in DB.sdvx_3s2_game_load(refid)) {
                game = new XElement("game_3");
                game.Add(new XElement("result", 0, new XAttribute("__type", "s8")));
                game.Add(new XElement("name", item["name"]));
                game.Add(new XElement("code", item["code"]));
                game.Add(new XElement("creator_id", 0, new XAttribute("__type", "u32")));
                XElement last = new XElement("last");
                last.Add(new XElement("music_id", item["last_music_id"], new XAttribute("__type", "s32")));
                last.Add(new XElement("music_type", item["last_music_type"], new XAttribute("__type", "u8")));
                last.Add(new XElement("sort_type", item["sort_type"], new XAttribute("__type", "u8")));
                last.Add(new XElement("narrow_down", item["narrow_down"], new XAttribute("__type", "u8")));
                last.Add(new XElement("headphone", 0, new XAttribute("__type", "u8")));
                last.Add(new XElement("appeal_id", item["appeal_id"], new XAttribute("__type", "u16")));
                last.Add(new XElement("comment_id", item["comment_id"], new XAttribute("__type", "u16")));
                last.Add(new XElement("gauge_option", item["gauge_option"], new XAttribute("__type", "u8")));
                game.Add(last);
                game.Add(new XElement("skill_level", item["skill_level"], new XAttribute("__type", "s16")));
                game.Add(new XElement("skill_name_id", item["skill_name_id"], new XAttribute("__type", "s16")));
                game.Add(new XElement("hidden_param", item["hidden_param"], new XAttribute("__type", "s32"),
                    new XAttribute("__count", "20")));
                game.Add(new XElement("play_count", item["play_count"], new XAttribute("__type", "u32")));
                game.Add(new XElement("daily_count", item["daily_count"], new XAttribute("__type", "u32")));
                game.Add(new XElement("play_chain", item["play_chain"], new XAttribute("__type", "u32")));
                game.Add(new XElement("creator_item"));
                game.Add(new XElement("floorinfection"));
                game.Add(new XElement("pb"));
                game.Add(new XElement("blaster_energy", item["blaster_energy"], new XAttribute("__type", "u32")));
                game.Add(new XElement("blaster_count", item["blaster_count"], new XAttribute("__type", "u32")));
                game.Add(new XElement("pbr_infection"));
                game.Add(new XElement("eaappli", new XElement("relation", 0, new XAttribute("__type", "s8"))));
                XElement eashop = new XElement("ea_shop");
                eashop.Add(new XElement("packet_booster", item["packet_booster"], new XAttribute("__type", "s32")));
                eashop.Add(new XElement("block_booster", item["block_booster"], new XAttribute("__type", "s32")));
                game.Add(eashop);
                XElement story = new XElement("story");
                foreach (IDataRecord s in DB.sdvx_3s2_load_story(refid)) {
                    XElement info = new XElement("info");
                    info.Add(new XElement("story_id", s["story_id"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("progress_id", s["progress_id"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("progress_param", s["progress_param"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("clear_cnt", s["clear_cnt"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("route_flg", s["route_flg"], new XAttribute("__type", "u32")));
                    story.Add(info);
                }
                game.Add(story);
                game.Add(new XElement("gamecoin_packet", item["packet"], new XAttribute("__type", "u32")));
                game.Add(new XElement("gamecoin_block", item["block"], new XAttribute("__type", "u32")));
                XElement pitem = new XElement("item");
                foreach (IDataRecord s in DB.sdvx_3s2_load_item(refid)) {
                    XElement info = new XElement("info");
                    info.Add(new XElement("type", s["type"], new XAttribute("__type", "u8")));
                    info.Add(new XElement("id", s["id"], new XAttribute("__type", "u32")));
                    info.Add(new XElement("param", s["param"], new XAttribute("__type", "u32")));
                    pitem.Add(info);
                }
                game.Add(pitem);
                XElement param = new XElement("param");
                if (item["param_type"].ToString() != "0") {
                    param.Add(new XElement("info",
                        new XElement("type", item["param_type"], new XAttribute("__type", "s32")),
                        new XElement("id", item["param_id"], new XAttribute("__type", "s32")),
                        new XElement("param", item["param"], new XAttribute("__type", "s32"),
                            new XAttribute("__count", "6"))
                    ));
                }
                game.Add(param);
                XElement skill = new XElement("skill");
                XElement course = new XElement("course_all");
                foreach (IDataRecord item2 in DB.sdvx_3s2_load_skill_log(refid)) {
                    XElement d = new XElement("d");
                    d.Add(new XElement("crsid", item2["crsid"], new XAttribute("__type", "s16")));
                    d.Add(new XElement("ct", item2["ct"], new XAttribute("__type", "s16")));
                    d.Add(new XElement("ar", item2["ar"], new XAttribute("__type", "s16")));
                    d.Add(new XElement("ssnid", item2["ssnid"], new XAttribute("__type", "s32")));
                    course.Add(d);
                }
                skill.Add(course);
                game.Add(skill);
                doc = new XDocument(new XElement("response", game));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
            game = new XElement("game_3", new XElement("result", 1, new XAttribute("__type", "s8")));
            doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_loadm() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/game_3/dataid").Value;
            XElement game = new XElement("game_3");
            XElement @new = new XElement("new");
            foreach (IDataRecord record in DB.sdvx_3s2_load_music(refid)) {
                XElement music = new XElement("music");
                music.Add(new XElement("music_id", record["music_id"], new XAttribute("__type", "u32")));
                music.Add(new XElement("music_type", record["music_type"], new XAttribute("__type", "u32")));
                music.Add(new XElement("score", record["score"], new XAttribute("__type", "u32")));
                music.Add(new XElement("clear_type", record["clear_type"], new XAttribute("__type", "u32")));
                music.Add(new XElement("cnt", record["cnt"], new XAttribute("__type", "u32")));
                music.Add(new XElement("score_grade", record["score_grade"], new XAttribute("__type", "u32")));
                music.Add(new XElement("btn_rate", record["btn_rate"], new XAttribute("__type", "u32")));
                music.Add(new XElement("long_rate", record["long_rate"], new XAttribute("__type", "u32")));
                music.Add(new XElement("vol_rate", record["vol_rate"], new XAttribute("__type", "u32")));
                @new.Add(music);
            }
            game.Add(@new, new XElement("old"));
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_loadr() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_frozen() {
            XElement game = new XElement("game_3");
            game.Add(new XElement("result", 0, new XAttribute("__type", "u8")));
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_new() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/game_3/refid").Value;
            string name = req.XPathSelectElement("/call/game_3/name").Value;
            DB.sdvx_3s2_new_profile(refid, name);
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_lounge() {
            XElement game = new XElement("game_3");
            game.Add(new XElement("interval", 10, new XAttribute("__type", "u32")));
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_entry_s() {
            XElement game = new XElement("game_3");
            Random rand = new Random();
            game.Add(new XElement("entry_id", rand.Next(2000000000), new XAttribute("__type", "u32")));
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_entry_e() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_savem() {
            XDocument req = parse_request();
            XElement game = req.XPathSelectElement("/call/game_3");
            string refid = game.Element("refid")?.Value;
            string music_id = game.Element("music_id")?.Value;
            string music_type = game.Element("music_type")?.Value;
            string score = game.Element("score")?.Value;
            string clear_type = game.Element("clear_type")?.Value;
            string score_grade = game.Element("score_grade")?.Value;
            string btn_rate = game.Element("btn_rate")?.Value;
            string long_rate = game.Element("long_rate")?.Value;
            string vol_rate = game.Element("vol_rate")?.Value;
            string max_chain = game.Element("max_chain")?.Value;
            string critical = game.Element("critical")?.Value;
            string near = game.Element("near")?.Value;
            string error = game.Element("error")?.Value;
            string effective_rate = game.Element("effective_rate")?.Value;
            string mode = game.Element("mode")?.Value;
            string gauge_type = game.Element("gauge_type")?.Value;
            string drop_frame = game.Element("drop_frame")?.Value;
            string drop_frame_max = game.Element("drop_frame_max")?.Value;
            string drop_count = game.Element("drop_count")?.Value;
            string locid = game.Element("locid")?.Value;
            RecordSDVXMusic record = new RecordSDVXMusic {
                music_id = music_id,
                music_type = music_type,
                score = score,
                clear_type = clear_type,
                score_grade = score_grade,
                btn_rate = btn_rate,
                long_rate = long_rate,
                vol_rate = vol_rate,
                max_chain = max_chain,
                critical = critical,
                near = near,
                error = error,
                effective_rate = effective_rate,
                mode = mode,
                gauge_type = gauge_type,
                drop_frame = drop_frame,
                drop_frame_max = drop_frame_max,
                drop_count = drop_count,
                locid = locid
            };
            DB.sdvx_3s2_save_music(refid, record);
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_savee() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_savec() {
            XDocument req = parse_request();
            XElement game = req.XPathSelectElement("/call/game_3");
            string refid = game.Element("dataid")?.Value;
            string crsid = game.Element("crsid")?.Value;
            string ct = game.Element("ct")?.Value;
            string ar = game.Element("ar")?.Value;
            string ssnid = game.Element("ssnid")?.Value;
            RecordSDVXCourse record = new RecordSDVXCourse {
                crsid = crsid,
                ct = ct,
                ar = ar,
                ssnid = ssnid
            };
            DB.sdvx_3s2_save_user_course(refid, record);
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_buy() {
            XDocument req = parse_request();
            XElement item = req.XPathSelectElement("/call/game_3/item");
            string refid = req.XPathSelectElement("/call/game_3/refid").Value;
            string item_type = item.Element("item_type")?.Value;
            string item_id = item.Element("item_id")?.Value;
            string param = item.Element("param")?.Value;
            string price = item.Element("price")?.Value;
            string currency_type = req.XPathSelectElement("/call/game_3/currency_type").Value;
            RecordSDVXBuy record = new RecordSDVXBuy {
                item_type = item_type,
                item_id = item_id,
                param = param,
                price = price,
                currency_type = currency_type
            };
            object packet;
            object block;
            DB.sdvx_3s2_user_buy(refid, record, out packet, out block);
            XElement game = new XElement("game_3");
            game.Add(new XElement("gamecoin_packet", packet, new XAttribute("__type", "u32")));
            game.Add(new XElement("gamecoin_block", block, new XAttribute("__type", "u32")));
            game.Add(new XElement("result", 0, new XAttribute("__type", "u38")));
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_save() {
            XDocument req = parse_request();
            XElement game = req.XPathSelectElement("/call/game_3");
            string refid = game.Element("refid")?.Value;
            string appeal_id = game.Element("appeal_id")?.Value;
            string comment_id = game.Element("comment_id")?.Value;
            string music_id = game.Element("music_id")?.Value;
            string music_type = game.Element("music_type")?.Value;
            string sort_type = game.Element("sort_type")?.Value;
            string narrow_down = game.Element("narrow_down")?.Value;
            string gauge_option = game.Element("gauge_option")?.Value;
            string earned_gamecoin_packet = game.Element("earned_gamecoin_packet")?.Value;
            string earned_gamecoin_block = game.Element("earned_gamecoin_block")?.Value;
            XElement paraminfo = game.Element("param")?.Element("info");
            string type = paraminfo?.Element("type")?.Value;
            string id = paraminfo?.Element("id")?.Value;
            string param = paraminfo?.Element("param")?.Value;
            string hidden_param = game.Element("hidden_param")?.Value;
            string skill_name_id = game.Element("skill_name_id")?.Value;
            string earned_blaster_energy = game.Element("earned_blaster_energy")?.Value;
            string blaster_count = game.Element("blaster_count")?.Value;
            string used_packet_booster = game.Element("ea_shop").Element("used_packet_booster")?.Value;
            string used_block_booster = game.Element("ea_shop").Element("used_block_booster")?.Value;
            RecordSDVXUser user = new RecordSDVXUser {
                appeal_id = appeal_id,
                comment_id = comment_id,
                music_id = music_id,
                music_type = music_type,
                sort_type = sort_type,
                narrow_down = narrow_down,
                gauge_option = gauge_option,
                earned_gamecoin_packet = earned_gamecoin_packet,
                earned_gamecoin_block = earned_gamecoin_block,
                type = type,
                id = id,
                param = param,
                hidden_param = hidden_param,
                skill_name_id = skill_name_id,
                earned_blaster_energy = earned_blaster_energy,
                blaster_count = blaster_count,
                used_packet_booster = used_packet_booster,
                used_block_booster = used_block_booster
            };
            DB.sdvx_3s2_save_user(refid, user);
            IEnumerable<XElement> story = req.XPathSelectElements("/call/game_3/story/info");
            foreach (XElement item in story) {
                string story_id = item.Element("story_id").Value;
                string progress_id = item.Element("progress_id").Value;
                string progress_param = item.Element("progress_param").Value;
                string clear_cnt = item.Element("clear_cnt").Value;
                string route_flg = item.Element("route_flg").Value;
                RecordSDVXStory s = new RecordSDVXStory {
                    story_id = story_id,
                    progress_id = progress_id,
                    progress_param = progress_param,
                    clear_cnt = clear_cnt,
                    route_flg = route_flg
                };
                DB.sdvx_3s2_save_user_story(refid, s);
            }
            IEnumerable<XElement> it = req.XPathSelectElements("/call/game_3/item/info");
            foreach (XElement item in it) {
                string itemid = item.Element("id").Value;
                string itemtype = item.Element("type").Value;
                string itemparam = item.Element("param").Value;
                RecordSDVXItem i = new RecordSDVXItem {
                    id = itemid,
                    type = itemtype,
                    param = itemparam
                };
                DB.sdvx_3s2_save_user_item(refid, i);
            }
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] sdvx_3s2_game_playe() {
            XDocument doc = new XDocument(new XElement("response", new XElement("game_3")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_print() {
            XDocument req = parse_request();
            XElement card = req.XPathSelectElement("/call/game_3/genesis_card");
            XElement game = new XElement("game_3");
            XElement gene = new XElement("genesis_card");
            gene.Add(new XElement("index", card.Element("index")?.Value, new XAttribute("__type", "s32")));
            gene.Add(new XElement("print_id", card.Element("print_id")?.Value, new XAttribute("__type", "s32")));
            game.Add(gene);
            XElement ticket = new XElement("after_ticket");
            ticket.Add(new XElement("ticket_id", card.Element("ticket_id")?.Value, new XAttribute("__type", "s32")));
            ticket.Add(new XElement("param", 0, new XAttribute("__type", "s32")));
            game.Add(ticket);
            XElement power = new XElement("after_power");
            power.Add(new XElement("generator_id", card.Element("generator_id")?.Value,
                new XAttribute("__type", "s32")));
            power.Add(new XElement("param", 0, new XAttribute("__type", "s32")));
            game.Add(power);
            XDocument doc = new XDocument(new XElement("response", game));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] sdvx_3s2_game_printh() {
            XDocument req = parse_request();
            XElement game = req.XPathSelectElement("/call/game_3");
            XElement game3 = new XElement("game_3");
            XElement card = new XElement("genesis_card");
            for (int i = 1; i <= 10; i++) {
                XElement history = new XElement("history");
                history.Add(new XElement("print_id", i, new XAttribute("__type", "s32")));
                history.Add(new XElement("card_id", i, new XAttribute("__type", "s32")));
                history.Add(new XElement("remain", 1, new XAttribute("__type", "s32")));
                history.Add(new XElement("print_time", 1495049930000 - i * 90000, new XAttribute("__type", "u64")));
                card.Add(history);
            }
            game3.Add(card);
            XDocument doc = new XDocument(new XElement("response", game3));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion

        #region IIDX 24

        byte[] iidx24_pc_common() {
            XElement root = new XElement("IIDX24pc");
            root.Add(new XElement("monthly_mranking",
                "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
                new XAttribute("__type", "u16"),
                new XAttribute("__count", "40")
            ));
            root.Add(new XElement("total_mranking",
                "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
                new XAttribute("__type", "u16"),
                new XAttribute("__count", "40")
            ));
            root.Add(new XElement("ir", new XAttribute("beat", "10")));
            root.Add(new XElement("license", new XElement("string", "10", new XAttribute("__type", "bin"))));
            root.Add(new XElement("expert", new XAttribute("phase", "1")));
            root.Add(new XElement("boss", new XAttribute("phase", "1")));
            root.Add(new XElement("extra_boss_event", new XAttribute("phase", "1")));
            root.Add(new XElement("newsong_another", new XAttribute("open", "1")));
            root.Add(new XElement("event1_phase", new XAttribute("phase", "1")));
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] iidx24_shop_getname() {
            XElement root = new XElement("IIDX24shop");
            root.Add(new XAttribute("opname", "opname"));
            root.Add(new XAttribute("pid", 1));
            root.Add(new XAttribute("cls_opt", 0));
            root.Add(new XAttribute("hr", 0));
            root.Add(new XAttribute("mi", 0));
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion

        #region BeatStream 2

        static byte[] bst_2_pcb2_error() {
            XDocument doc = new XDocument(new XElement("response", new XElement("pcb2")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_pcb2_boot() {
            XDocument doc = new XDocument(
                new XElement("response",
                    new XElement("pcb2",
                        new XElement("sinfo",
                            new XElement("nm", "1"),
                            new XElement("cl_enbl", 1, new XAttribute("__type", "bool")),
                            new XElement("cl_h", 1, new XAttribute("__type", "u8")),
                            new XElement("cl_m", 1, new XAttribute("__type", "u8"))
                        )
                    )
                )
            );
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_pcb2_uptime_update() {
            XDocument doc = new XDocument(new XElement("response", new XElement("pcb2")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_shop2_setting_write() {
            XDocument doc = new XDocument(new XElement("response", new XElement("shop2")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_info2_common() {
            XElement root = new XElement("info2");
            XElement event_ctrl = new XElement("event_ctrl");
            foreach (int i in Enumerable.Range(0, 21)) {
                foreach (int j in Enumerable.Range(0, 21)) {
                    XElement data = new XElement("data",
                        new XElement("type", i, new XAttribute("__type", "s32")),
                        new XElement("phase", j, new XAttribute("__type", "s32"))
                    );
                    event_ctrl.Add(data);
                }
            }
            root.Add(event_ctrl);
            XElement music_list = new XElement("music_list");
            foreach (int i in Enumerable.Range(0, 273)) {
                XElement data = new XElement("data",
                    new XElement("music_id", i, new XAttribute("__type", "s32")),
                    new XElement("ins_music_id", i, new XAttribute("__type", "s32")),
                    new XElement("is_play", 1, new XAttribute("__type", "bool"))
                );
                music_list.Add(data);
            }
            root.Add(music_list);
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_info2_music_count_read() {
            XElement record = new XElement("record");
            foreach (IDataRecord i in DB.bst_2_get_music_record()) {
                XElement rate = new XElement("rate");
                rate.Add(new XElement("music_id", i["music_id"], new XAttribute("__type", "s32")));
                rate.Add(new XElement("note_level", i["note_level"], new XAttribute("__type", "s32")));
                rate.Add(new XElement("play_count", i["play_count"], new XAttribute("__type", "s32")));
                rate.Add(new XElement("clear_count", i["clear_count"], new XAttribute("__type", "s32")));
                record.Add(rate);
            }
            XElement root = new XElement("info2", record);
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        static byte[] bst_2_info2_music_ranking_read() {
            XElement ranking = new XElement("ranking");
            foreach (IDataRecord i in DB.bst_2_get_music_ranking()) {
                XElement rate = new XElement("rate");
                rate.Add(new XElement("music_id", i["music_id"], new XAttribute("__type", "s32")));
                rate.Add(new XElement("play_count", i["play_count"], new XAttribute("__type", "s32")));
                ranking.Add(rate);
            }
            XElement root = new XElement("info2", ranking);
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_start() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/player2/rid").Value;
            XElement root = new XElement("player2");
            if (refid == "") {
                root.Add(new XElement("plyid", 1, new XAttribute("__type", "s32")));
                root.Add(new XElement("start_time",
                    (int) ((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds),
                    new XAttribute("__type", "u64")));
                root.Add(new XElement("event_ctrl"));
                root.Add(
                    new XElement("floor_infection",
                        new XElement("event",
                            new XElement("infection_id", 0, new XAttribute("__type", "s32")),
                            new XElement("music_list", 0, new XAttribute("__type", "s32")),
                            new XElement("is_complete", 0, new XAttribute("__type", "bool"))
                        )
                    )
                );
                root.Add(
                    new XElement("museca",
                        new XElement("is_play_museca", 0, new XAttribute("__type", "bool"))
                    )
                );
            } else {
                foreach (IDataRecord data in DB.bst_2_get_player_start(refid)) {
                    root.Add(new XElement("plyid", data["plyid"], new XAttribute("__type", "s32")));
                    root.Add(new XElement("start_time",
                        (int) ((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds),
                        new XAttribute("__type", "u64")));
                    root.Add(new XElement("event_ctrl"));
                    root.Add(
                        new XElement("floor_infection",
                            new XElement("event",
                                new XElement("infection_id", data["fi_id"], new XAttribute("__type", "s32")),
                                new XElement("music_list", data["fi_music_list"], new XAttribute("__type", "s32")),
                                new XElement("is_complete", data["fi_is_complete"], new XAttribute("__type", "bool"))
                            )
                        )
                    );
                    root.Add(
                        new XElement("museca",
                            new XElement("is_play_museca", data["is_play_museca"], new XAttribute("__type", "bool"))
                        )
                    );
                }
            }
            XElement music_list = new XElement("music_list");
            foreach (int i in Enumerable.Range(0, 273)) {
                XElement mdata = new XElement("data",
                    new XElement("music_id", i, new XAttribute("__type", "s32")),
                    new XElement("ins_music_id", i, new XAttribute("__type", "s32")),
                    new XElement("is_play", 1, new XAttribute("__type", "bool"))
                );
                music_list.Add(mdata);
            }
            root.Add(music_list);
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_succeed() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/player2/rid").Value;

            XElement root = new XElement("player2",
                new XElement("play", 0, new XAttribute("__type", "bool")),
                new XElement("data",
                    new XElement("name", "", new XAttribute("__type", "str"))
                ),
                new XElement("record"),
                new XElement("hacker"),
                new XElement("phantom")
            );
            XDocument doc = new XDocument(new XElement("response", root));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_write() {
            XDocument req = parse_request();
            XElement pdata = req.XPathSelectElement("/call/player2/pdata");
            XElement account = pdata.Element("account");
            string usrid = account.Element("usrid").Value;
            bool create = false;
            if (account.Element("rid").Value == "") {
                goto skip;
            }
            if (usrid == "0") {
                usrid = new Random().Next().ToString();
                create = true;
            }
            RecordBSTAccount a = new RecordBSTAccount {
                usrid = usrid,
                is_takeover = account.Element("is_takeover").Value,
                tpc = account.Element("tpc").Value,
                dpc = account.Element("dpc").Value,
                crd = account.Element("crd").Value,
                brd = account.Element("brd").Value,
                tdc = account.Element("tdc").Value,
                rid = account.Element("rid").Value,
                lid = account.Element("lid").Value,
                mode = account.Element("mode").Value,
                ver = account.Element("ver").Value,
                pp = account.Element("pp").Value,
                ps = account.Element("ps").Value,
                pay = account.Element("pay").Value,
                pay_pc = account.Element("pay_pc").Value,
                st = account.Element("st").Value
            };
            XElement _base = pdata.Element("base");
            RecordBSTBase b = new RecordBSTBase {
                name = _base.Element("name").Value,
                brnk = _base.Element("brnk").Value,
                bcnum = _base.Element("bcnum").Value,
                lcnum = _base.Element("lcnum").Value,
                volt = _base.Element("volt").Value,
                gold = _base.Element("gold").Value,
                lmid = _base.Element("lmid").Value,
                lgrd = _base.Element("lgrd").Value,
                lsrt = _base.Element("lsrt").Value,
                ltab = _base.Element("ltab").Value,
                splv = _base.Element("splv").Value,
                pref = _base.Element("pref").Value,
                lcid = _base.Element("lcid").Value,
                hat = _base.Element("hat").Value
            };
            string custom = pdata.XPathSelectElement("customize/custom").Value;
            string last_tips = pdata.XPathSelectElement("tips/last_tips").Value;
            if (create) {
                DB.bst_2_create_player(a, b, custom, last_tips);
            } else {
                DB.bst_2_write_player(a, b, custom, last_tips);
            }
            XElement item = pdata.Element("item");
            IEnumerable<XElement> info = item.Elements("info");
            foreach (XElement i in info) {
                RecordBSTItem it = new RecordBSTItem {
                    type = i.Element("type").Value,
                    id = i.Element("id").Value,
                    param = i.Element("param").Value,
                    count = i.Element("count").Value
                };
                DB.bst_2_write_player_item(account.Element("rid").Value, it);
            }
            skip:
            XDocument doc = new XDocument(
                new XElement("response",
                    new XElement("player2",
                        new XElement("uid", usrid, new XAttribute("__type", "s32"))
                    )
                )
            );
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_read() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/player2/rid").Value;
            XDocument doc;
            foreach (IDataRecord d in DB.bst_2_get_player_data(refid)) {
                XElement pdata = new XElement("pdata");
                XElement account = new XElement("account");
                account.Add(new XElement("usrid", d["usrid"], new XAttribute("__type", "s32")));
                account.Add(new XElement("is_takeover", d["is_takeover"], new XAttribute("__type", "s32")));
                account.Add(new XElement("tpc", d["tpc"], new XAttribute("__type", "s32")));
                account.Add(new XElement("dpc", d["dpc"], new XAttribute("__type", "s32")));
                account.Add(new XElement("crd", d["crd"], new XAttribute("__type", "s32")));
                account.Add(new XElement("brd", d["brd"], new XAttribute("__type", "s32")));
                account.Add(new XElement("tdc", d["tdc"], new XAttribute("__type", "s32")));
                account.Add(new XElement("intrvld", 1, new XAttribute("__type", "s32")));
                account.Add(new XElement("ver", d["ver"], new XAttribute("__type", "s16")));
                account.Add(new XElement("pst", d["st"], new XAttribute("__type", "u64")));
                account.Add(new XElement("st", (long) ((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds * 1000), new XAttribute("__type", "u64")));
                account.Add(new XElement("ea", 1, new XAttribute("__type", "bool")));
                pdata.Add(account);
                XElement _base = new XElement("base");
                _base.Add(new XElement("name", d["name"]));
                _base.Add(new XElement("brnk", d["brnk"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("bcnum", d["bcnum"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("lcnum", d["lcnum"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("volt", d["volt"], new XAttribute("__type", "s32")));
                _base.Add(new XElement("gold", d["gold"], new XAttribute("__type", "s32")));
                _base.Add(new XElement("lmid", d["lmid"], new XAttribute("__type", "s32")));
                _base.Add(new XElement("lgrd", d["lgrd"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("lsrt", d["lsrt"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("ltab", d["ltab"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("splv", d["splv"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("pref", d["pref"], new XAttribute("__type", "s8")));
                _base.Add(new XElement("lcid", d["lcid"], new XAttribute("__type", "s32")));
                _base.Add(new XElement("hat", d["hat"], new XAttribute("__type", "s32")));
                pdata.Add(_base);
                XElement item = new XElement("item");
                foreach (IDataRecord i in DB.bst_2_get_player_item(refid)) {
                    XElement info = new XElement("info");
                    info.Add(new XElement("type", i["type"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("id", i["id"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("param", i["param"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("count", i["count"], new XAttribute("__type", "s32")));
                    item.Add(info);
                }
                pdata.Add(item);
                pdata.Add(new XElement("customize",
                    new XElement("custom", d["custom"], new XAttribute("__type", "u16"))
                ));
                pdata.Add(new XElement("tips",
                    new XElement("last_tips", d["last_tips"], new XAttribute("__type", "s32"))
                ));
                XElement hacker = new XElement("hacker");
                foreach (IDataRecord i in DB.bst_2_get_hacker(refid)) {
                    XElement info = new XElement("info");
                    info.Add(new XElement("id", i["id"], new XAttribute("__type", "s32")));
                    info.Add(new XElement("state0", i["state0"], new XAttribute("__type", "s8")));
                    info.Add(new XElement("state1", i["state1"], new XAttribute("__type", "s8")));
                    info.Add(new XElement("state2", i["state2"], new XAttribute("__type", "s8")));
                    info.Add(new XElement("state3", i["state3"], new XAttribute("__type", "s8")));
                    info.Add(new XElement("state4", i["state4"], new XAttribute("__type", "s8")));
                    info.Add(new XElement("update_time", i["update_time"], new XAttribute("__type", "u64")));
                    hacker.Add(info);
                }
                pdata.Add(hacker);
                pdata.Add(new XElement("phantom",
                    new XElement("minfo",
                        new XElement("mid", 126, new XAttribute("__type", "s32")),
                        new XElement("cbit", 0, new XAttribute("__type", "s64")),
                        new XElement("clr", 0, new XAttribute("__type", "bool"))
                    )
                ));
                XElement record = new XElement("record");
                foreach (IDataRecord i in DB.bst_2_get_player_stage(refid)) {
                    XElement rec = new XElement("rec");
                    rec.Add(new XElement("music_id", i["music_id"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("note_level", i["note_level"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("play_count", i["play_count"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("clear_count", i["clear_count"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("best_gauge", i["best_gauge"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("best_score", i["best_score"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("best_grade", i["best_grade"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("best_medal", i["best_medal"], new XAttribute("__type", "s32")));
                    record.Add(rec);
                }
                foreach (IDataRecord i in DB.bst_2_get_music_record()) {
                    XElement rate = new XElement("rate");
                    rate.Add(new XElement("music_id", i["music_id"], new XAttribute("__type", "s32")));
                    rate.Add(new XElement("note_level", i["note_level"], new XAttribute("__type", "s32")));
                    rate.Add(new XElement("play_count", i["play_count"], new XAttribute("__type", "s32")));
                    rate.Add(new XElement("clear_count", i["clear_count"], new XAttribute("__type", "s32")));
                    record.Add(rate);
                }
                pdata.Add(record);
                XElement course = new XElement("course");
                foreach (IDataRecord i in DB.bst_2_get_player_course(refid)) {
                    XElement rec = new XElement("rec");
                    rec.Add(new XElement("course_id", i["course_id"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("play", d["usrid"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("is_touch", 1, new XAttribute("__type", "bool")));
                    rec.Add(new XElement("clear", i["medal"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("gauge", i["gauge"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("score", i["score"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("grade", i["grade"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("medal", i["medal"], new XAttribute("__type", "s32")));
                    rec.Add(new XElement("combo", i["combo"], new XAttribute("__type", "s32")));
                    course.Add(rec);
                }
                foreach (IDataRecord i in DB.bst_2_get_course_record()) {
                    XElement rate = new XElement("rate");
                    rate.Add(new XElement("course_id", i["course_id"], new XAttribute("__type", "s32")));
                    rate.Add(new XElement("play_count", i["play_count"], new XAttribute("__type", "s32")));
                    rate.Add(new XElement("clear_count", i["clear_count"], new XAttribute("__type", "s32")));
                    course.Add(rate);
                }
                pdata.Add(course);
                doc = new XDocument(new XElement("response", new XElement("player2", pdata)));
                return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
            }
            XElement player2 = new XElement("player2", new XElement("result", 1, new XAttribute("__type", "s8")));
            doc = new XDocument(new XElement("response", player2));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_stagedata_write() {
            XDocument req = parse_request();
            XElement data = req.XPathSelectElement("/call/player2");
            RecordBSTStage s = new RecordBSTStage {
                user_id = data.Element("user_id").Value,
                location_id = data.Element("location_id").Value,
                select_music_id = data.Element("select_music_id").Value,
                select_grade = data.Element("select_grade").Value,
                result_clear_gauge = data.Element("result_clear_gauge").Value,
                result_score = data.Element("result_score").Value,
                result_max_combo = data.Element("result_max_combo").Value,
                result_grade = data.Element("result_grade").Value,
                result_medal = data.Element("result_medal").Value,
                result_fanta = data.Element("result_fanta").Value,
                result_great = data.Element("result_great").Value,
                result_fine = data.Element("result_fine").Value,
                result_miss = data.Element("result_miss").Value
            };
            DB.bst_2_write_stage(s);
            XDocument doc = new XDocument(
                new XElement("response", new XElement("player2"))
            );
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }
        
        byte[] bst_2_player2_course_stage_data_write() {
            XDocument req = parse_request();
            XElement data = req.XPathSelectElement("/call/player2");
            RecordBSTStage s = new RecordBSTStage {
                user_id = data.Element("user_id").Value,
                location_id = "COURSE",
                select_music_id = data.Element("select_music_id").Value,
                select_grade = data.Element("select_grade").Value,
                result_clear_gauge = data.Element("result_clear_gauge").Value,
                result_score = data.Element("result_score").Value,
                result_max_combo = data.Element("result_max_combo").Value,
                result_grade = data.Element("result_grade").Value,
                result_medal = data.Element("result_medal").Value,
                result_fanta = data.Element("result_fanta").Value,
                result_great = data.Element("result_great").Value,
                result_fine = data.Element("result_fine").Value,
                result_miss = data.Element("result_miss").Value
            };
            DB.bst_2_write_stage(s);
            XDocument doc = new XDocument(
                new XElement("response", new XElement("player2"))
            );
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_course_data_write() {
            XDocument req = parse_request();
            XElement data = req.XPathSelectElement("/call/player2");
            RecordBSTCourse c = new RecordBSTCourse {
                user_id = data.Element("user_id").Value,
                course_id = data.Element("course_id").Value,
                gauge = data.Element("gauge").Value,
                score = data.Element("score").Value,
                grade = data.Element("grade").Value,
                medal = data.Element("medal").Value,
                combo = data.Element("combo").Value,
                fanta = data.Element("fanta").Value,
                great = data.Element("great").Value,
                fine = data.Element("fine").Value,
                miss = data.Element("miss").Value,
                lid = data.Element("lid").Value
            };
            DB.bst_2_write_course(c);
            XDocument doc = new XDocument(new XElement("response", new XElement("player2")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        byte[] bst_2_player2_end() {
            XDocument req = parse_request();
            string refid = req.XPathSelectElement("/call/player2/rid").Value;
            DB.bst_2_end_player(refid);
            XDocument doc = new XDocument(new XElement("response", new XElement("player2")));
            return Encoding.GetEncoding(Utils.m_encoding).GetBytes(doc.ToString());
        }

        #endregion
    }

    #region SDVX records

    public class RecordSDVXMusic {
        public string music_id;
        public string music_type;
        public string score;
        public string clear_type;
        public string score_grade;
        public string btn_rate;
        public string long_rate;
        public string vol_rate;
        public string max_chain;
        public string critical;
        public string near;
        public string error;
        public string effective_rate;
        public string mode;
        public string gauge_type;
        public string drop_frame;
        public string drop_frame_max;
        public string drop_count;
        public string locid;
    }

    public class RecordSDVXUser {
        public string appeal_id;
        public string comment_id;
        public string music_id;
        public string music_type;
        public string sort_type;
        public string narrow_down;
        public string gauge_option;
        public string earned_gamecoin_packet;
        public string earned_gamecoin_block;
        public string type;
        public string id;
        public string param;
        public string hidden_param;
        public string skill_name_id;
        public string earned_blaster_energy;
        public string blaster_count;
        public string used_packet_booster;
        public string used_block_booster;
    }

    public class RecordSDVXStory {
        public string story_id;
        public string progress_id;
        public string progress_param;
        public string clear_cnt;
        public string route_flg;
    }

    public class RecordSDVXItem {
        public string id;
        public string type;
        public string param;
    }

    public class RecordSDVXCourse {
        public string crsid;
        public string ct;
        public string ar;
        public string ssnid;
    }

    public class RecordSDVXBuy {
        public string item_type;
        public string item_id;
        public string param;
        public string price;
        public string currency_type;
    }

    #endregion

    #region BST records

    public class RecordBSTAccount {
        public string usrid;
        public string is_takeover;
        public string tpc;
        public string dpc;
        public string crd;
        public string brd;
        public string tdc;
        public string rid;
        public string lid;
        public string mode;
        public string ver;
        public string pp;
        public string ps;
        public string pay;
        public string pay_pc;
        public string st;
    }

    public class RecordBSTBase {
        public string name;
        public string brnk;
        public string bcnum;
        public string lcnum;
        public string volt;
        public string gold;
        public string lmid;
        public string lgrd;
        public string lsrt;
        public string ltab;
        public string splv;
        public string pref;
        public string lcid;
        public string hat;
    }

    public class RecordBSTItem {
        public string id;
        public string type;
        public string param;
        public string count;
    }

    public class RecordBSTStage {
        public string user_id;
        public string location_id;
        public string select_music_id;
        public string select_grade;
        public string result_clear_gauge;
        public string result_score;
        public string result_max_combo;
        public string result_grade;
        public string result_medal;
        public string result_fanta;
        public string result_great;
        public string result_fine;
        public string result_miss;
    }

    public class RecordBSTCourse {
        public string user_id;
        public string course_id;
        public string gauge;
        public string score;
        public string grade;
        public string medal;
        public string combo;
        public string fanta;
        public string great;
        public string fine;
        public string miss;
        public string lid;
    }

#endregion

}
