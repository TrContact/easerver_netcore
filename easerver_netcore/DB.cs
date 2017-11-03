using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace easerver_netcore {
    public static class DB {
        const string ConnStr = "server=127.0.0.1;user=ea;database=ea;port=3306;password=eamusement;";

        public static bool card_inquire(string cardid, out string dataid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    $"SELECT COUNT(cardid) AS count, dataid FROM card WHERE cardid = \"{cardid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (int.Parse(reader["count"].ToString()) == 1) {
                    dataid = reader["dataid"].ToString();
                    return true;
                }
                dataid = "";
                return false;
            }
        }

        public static bool card_binded(string model, string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT COUNT(refid) AS count FROM card_bind WHERE refid = \"{refid}\" AND model = \"{model}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return int.Parse(reader["count"].ToString()) == 1;
            }
        }

        public static void card_bind(string model, string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"INSERT INTO card_bind VALUES (\"{refid}\", \"{model}\")";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool card_auth(string refid, string passwd) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    $"SELECT COUNT(cardid) AS count FROM card WHERE dataid = \"{refid}\" AND pass = \"{passwd}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return int.Parse(reader["count"].ToString()) == 1;
            }
        }
        
        public static void card_create(string cardid, string dataid, string passwd) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"INSERT INTO card VALUES (\"{cardid}\", \"{dataid}\", 1, \"{passwd}\")";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = $"INSERT INTO eacoin_balance VALUES (\"{cardid}\", 1000)";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static object eacoin_balance(string cardid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT balance FROM eacoin_balance WHERE cardid = \"{cardid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return reader["balance"];
            }
        }

        public static int eacoin_consume(string cardid, int price, int service) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT balance FROM eacoin_balance WHERE cardid = \"{cardid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int balance = Convert.ToInt32(reader["balance"].ToString());
                if (balance - price < 500) {
                    price -= 1000;
                }
                reader.Close();
                query = $"UPDATE eacoin_balance SET balance = balance - {price} WHERE cardid = \"{cardid}\"";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = $"INSERT INTO eacoin_log (consume, service, cardno) VALUES ({price}, {service}, \"{cardid}\")";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return balance - price;
            }
        }

        public static IEnumerable<IDataRecord> sdvx_3s2_get_music_list() {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = "SELECT music_id, difficulty FROM sdvx_music_difficulty_3s2";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> sdvx_3s2_get_skill_courses() {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = "SELECT * FROM sdvx_skill_3s2";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> sdvx_3s2_game_load(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM sdvx_user_3s2 WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> sdvx_3s2_load_story(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM sdvx_user_story_3s2 WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }
        
        public static IEnumerable<IDataRecord> sdvx_3s2_load_item(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM sdvx_user_item_3s2 WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }
        
        public static IEnumerable<IDataRecord> sdvx_3s2_load_music(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM sdvx_music_record_3s2 WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> sdvx_3s2_load_skill_log(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT * FROM sdvx_user_courses_3s2 WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static void sdvx_3s2_new_profile(string refid, string name) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                Random rand = new Random();
                int code = rand.Next(100000000);
                string query = $"INSERT INTO sdvx_user_3s2 VALUES (\"{refid}\", \"{name}\", \"{code:00000000}\", -1, 0, 0, 0, 0, 0, 0, 0, 0, \"0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, \"0 0 0 0 0 0\")";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void sdvx_3s2_save_music(string refid, RecordSDVXMusic r) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT * FROM sdvx_music_record_3s2 WHERE refid = \"{refid}\" AND music_id = {r.music_id} AND music_type = {r.music_type}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    int clear_type = Convert.ToInt32(reader["clear_type"].ToString());
                    if (Convert.ToInt32(r.clear_type) > clear_type) {
                        clear_type = Convert.ToInt32(r.clear_type);
                    }
                    int score_grade = Convert.ToInt32(reader["score_grade"].ToString());
                    if (Convert.ToInt32(r.score_grade) > score_grade) {
                        score_grade = Convert.ToInt32(r.score_grade);
                    }
                    int score = Convert.ToInt32(reader["score"].ToString());
                    using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                        conn2.Open();
                        if (Convert.ToInt32(r.score) > score) {
                            string q = "UPDATE sdvx_music_record_3s2 SET " +
                                       $"score = {r.score}, " +
                                       $"clear_type = {clear_type}, " +
                                       "cnt = cnt + 1, " +
                                       $"score_grade = {score_grade}, " +
                                       $"btn_rate = {r.btn_rate}, " +
                                       $"vol_rate = {r.vol_rate}, " +
                                       $"long_rate = {r.long_rate}, " +
                                       $"max_chain = {r.max_chain}, " +
                                       $"critical = {r.critical}, " +
                                       $"near = {r.near}, " +
                                       $"error = {r.error}, " +
                                       $"effective_rate = {r.effective_rate}, " +
                                       $"mode = {r.mode}, " +
                                       $"gauge_type = {r.gauge_type}, " +
                                       $"drop_frame = {r.drop_frame}, " +
                                       $"drop_frame_max = {r.drop_frame_max}, " +
                                       $"drop_count = {r.drop_count}, " +
                                       $"locid = \"{r.locid}\" " +
                                       $"WHERE refid = \"{refid}\" AND music_id = {r.music_id} AND music_type = {r.music_type}";
                            MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                            cmd2.ExecuteNonQuery();
                        } else {
                            string q = "UPDATE sdvx_music_record_3s2 SET " +
                                       $"clear_type = {clear_type}, " +
                                       "cnt = cnt + 1, " +
                                       $"score_grade = {score_grade}, " +
                                       $"WHERE refid = \"{refid}\" AND music_id = {r.music_id} AND music_type = {r.music_type}";
                            MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                } else {
                    using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                        conn2.Open();
                        string q =
                            "INSERT INTO sdvx_music_record_3s2 VALUES (" +
                            $"\"{refid}\", " +
                            $"{r.music_id}, " +
                            $"{r.music_type}, " +
                            $"{r.score}, " +
                            $"{r.clear_type}, " +
                            "1, " +
                            $"{r.score_grade}, " +
                            $"{r.btn_rate}, " +
                            $"{r.long_rate}, " +
                            $"{r.vol_rate}, " +
                            $"{r.max_chain}, " +
                            $"{r.critical}, " +
                            $"{r.near}, " +
                            $"{r.error}, " +
                            $"{r.effective_rate}, " +
                            $"{r.mode}, " +
                            $"{r.gauge_type}, " +
                            $"{r.drop_frame}, " +
                            $"{r.drop_frame_max}, " +
                            $"{r.drop_count}, " +
                            $"\"{r.locid}\")";
                        MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                        cmd2.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void sdvx_3s2_save_user(string refid, RecordSDVXUser r) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string q = "UPDATE sdvx_user_3s2 SET " +
                           $"appeal_id = {r.appeal_id}, " +
                           $"comment_id = {r.comment_id}, " +
                           $"last_music_id = {r.music_id}, " +
                           $"last_music_type = {r.music_type}, " +
                           $"sort_type = {r.sort_type}, " +
                           $"narrow_down = {r.narrow_down}, " +
                           $"gauge_option = {r.gauge_option}, " +
                           $"packet = packet + {r.earned_gamecoin_packet}, " +
                           $"block = block + {r.earned_gamecoin_block}, " +
                           $"param_type = {r.type}, " +
                           $"param_id = {r.id}, " +
                           $"param = \"{r.param}\", " +
                           $"hidden_param = \"{r.hidden_param}\", " +
                           $"skill_name_id = {r.skill_name_id}, " +
                           $"blaster_energy = blaster_energy + {r.earned_blaster_energy}, " +
                           $"blaster_count = {r.blaster_count}, " +
                           $"packet_booster = packet_booster - {r.used_packet_booster}, " +
                           $"block_booster = block_booster - {r.used_block_booster}, " +
                           "play_count = play_count + 1 " +
                           $"WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void sdvx_3s2_save_user_story(string refid, RecordSDVXStory r) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    "INSERT INTO sdvx_user_story_3s2 VALUES (" +
                    $"\"{refid}\", {r.story_id}, {r.progress_id}, {r.progress_param}, {r.clear_cnt}, {r.route_flg}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    $"progress_id = {r.progress_id}, " +
                    $"progress_param = {r.progress_param}, " +
                    $"clear_cnt = {r.clear_cnt}, " +
                    $"route_flg = {r.route_flg}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void sdvx_3s2_save_user_item(string refid, RecordSDVXItem r) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    "INSERT INTO sdvx_user_item_3s2 VALUES (" +
                    $"\"{refid}\", {r.id}, {r.type}, {r.param}) " +
                    $"ON DUPLICATE KEY UPDATE param = {r.param}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void sdvx_3s2_save_user_course(string refid, RecordSDVXCourse r) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    "INSERT INTO sdvx_user_courses_3s2 VALUES (" +
                    $"\"{refid}\", {r.crsid}, {r.ct}, {r.ar}, {r.ssnid})";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                if (r.ct == "2") {
                    query = $"SELECT skill_level FROM sdvx_user_3s2 WHERE refid = \"{refid}\"";
                    cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    int level = Convert.ToInt32(reader["skill_level"].ToString());
                    reader.Close();
                    if (level < int.Parse(r.ct)) {
                        query = $"UPDATE sdvx_user_3s2 SET skill_level = {r.crsid} WHERE refid = \"{refid}\"";
                    }
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void sdvx_3s2_user_buy(string refid, RecordSDVXBuy r, out object packet, out object block) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                int[] type = r.item_type.Split(' ').Select(int.Parse).ToArray();
                int[] id = r.item_id.Split(' ').Select(int.Parse).ToArray();
                int[] param = r.param.Split(' ').Select(int.Parse).ToArray();
                int[] price = r.price.Split(' ').Select(int.Parse).ToArray();
                string query = "INSERT INTO sdvx_user_item_3s2 VALUES ";
                for (int i = 0; i < type.Length; i++) {
                    query += $"(\"{refid}\", {id[i]}, {type[i]}, {param[i]})";
                    if (i != type.Length - 1) {
                        query += ", ";
                    }
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                int deduct = price.Aggregate((s, n) => s + n);
                string currency = r.currency_type == "0" ? "packet" : "block";
                query = $"UPDATE sdvx_user_3s2 SET {currency} = {currency} - {deduct} WHERE refid = \"{refid}\"";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                query = $"SELECT packet, block FROM sdvx_user_3s2 WHERE refid = \"{refid}\"";
                cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                packet = reader["packet"];
                block = reader["block"];
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_player_start(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT * FROM bst_player2_start WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    yield return reader;
                } else {
                    using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                        conn2.Open();
                        string q = "INSERT INTO bst_player2_start (refid) VALUES (" +
                                   $"\"{refid}\")";
                        MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                        cmd2.ExecuteNonQuery();
                        cmd2 = new MySqlCommand(query, conn2);
                        MySqlDataReader reader2 = cmd2.ExecuteReader();
                        reader2.Read();
                        yield return reader2;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_player_data(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM bst_player2_data WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_player_item(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM bst_player2_item WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_hacker(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT id, state0, state1, state2, state3, state4, UNIX_TIMESTAMP(update_time) AS update_time FROM bst_player2_hacker WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_player_stage(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT usrid FROM bst_player2_data WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                string usrid = reader["usrid"].ToString();
                using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                    conn2.Open();
                    string q = "SELECT " +
                               "select_music_id AS music_id, " +
                               "select_grade AS note_level, " +
                               "COUNT(result_score) AS play_count, " +
                               "SUM(CASE WHEN result_medal > 1 THEN 1 ELSE 0 END) AS clear_count, " +
                               "MAX(result_clear_gauge) AS best_gauge, " +
                               "MAX(result_score) AS best_score, " +
                               "MAX(result_grade) AS best_grade, " +
                               "MAX(result_medal) AS best_medal " +
                               "FROM bst_player2_stagedata " +
                               "WHERE " +
                               $"user_id = {usrid} " +
                               "GROUP BY user_id, select_music_id, select_grade";
                    MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    while (reader2.Read()) {
                        yield return reader2;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_player_course(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = $"SELECT usrid FROM bst_player2_data WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                string usrid = reader["usrid"].ToString();
                using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                    conn2.Open();
                    string query2 = "SELECT " +
                                    "course_id, " +
                                    "MAX(gauge) AS gauge, " +
                                    "MAX(score) AS score, " +
                                    "MAX(grade) AS grade, " +
                                    "MAX(medal) AS medal, " +
                                    "MAX(combo) AS combo " +
                                    "FROM bst_player2_course_data " +
                                    "WHERE " +
                                    $"user_id = {usrid} " +
                                    "GROUP BY user_id, course_id";
                    MySqlCommand cmd2 = new MySqlCommand(query2, conn2);
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    while (reader2.Read()) {
                        yield return reader2;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_music_record() {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query2 =
                    "SELECT " +
                    "select_music_id AS music_id, " +
                    "select_grade AS note_level, " +
                    "COUNT(result_score) AS play_count, " +
                    "SUM(CASE WHEN result_medal > 1 THEN 1 ELSE 0 END) AS clear_count " +
                    "FROM bst_player2_stagedata " +
                    "GROUP BY select_music_id, select_grade";
                MySqlCommand cmd = new MySqlCommand(query2, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_music_ranking() {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query2 =
                    "SELECT " +
                    "select_music_id AS music_id, " +
                    "COUNT(result_score) AS play_count " +
                    "FROM bst_player2_stagedata " +
                    "GROUP BY select_music_id " +
                    "ORDER BY play_count LIMIT 10";
                MySqlCommand cmd = new MySqlCommand(query2, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static IEnumerable<IDataRecord> bst_2_get_course_record() {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query2 =
                    "SELECT " +
                    "course_id, " +
                    "COUNT(score) AS play_count, " +
                    "SUM(CASE WHEN grade > 1 THEN 1 ELSE 0 END) AS clear_count " +
                    "FROM bst_player2_course_data " +
                    "GROUP BY course_id";
                MySqlCommand cmd = new MySqlCommand(query2, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    yield return reader;
                }
            }
        }

        public static void bst_2_create_player(RecordBSTAccount a, RecordBSTBase b, string custom, string last_tips) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = "INSERT INTO bst_player2_data VALUES (" +
                               $"\"{a.rid}\", {a.usrid}, {a.is_takeover}, {a.tpc}, {a.dpc}, {a.crd}, {a.brd}, {a.tdc}, \"{a.lid}\", {a.mode}, {a.ver}, {a.pp}, {a.ps}, {a.pay}, {a.pay_pc}, {a.st}, " +
                               $"\"{b.name}\", {b.brnk}, {b.bcnum}, {b.lcnum}, {b.volt}, {b.gold}, {b.lmid}, {b.lgrd}, {b.lsrt}, {b.ltab}, {b.splv}, {b.pref}, {b.lcid}, {b.hat}, " +
                               $"\"{custom}\", {last_tips})";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                foreach (int i in Enumerable.Range(0, 61)) {
                    query =
                        $"INSERT INTO bst_player2_hacker (refid, id, state0, state1, state2, state3, state4) VALUES (\"{a.rid}\", {i}, 0, 0, 0, 0, 0)";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void bst_2_write_player(RecordBSTAccount a, RecordBSTBase b, string custom, string last_tips) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query = "UPDATE bst_player2_data SET " +
                               $"usrid = {a.usrid}, " +
                               $"is_takeover = {a.is_takeover}, " +
                               $"tpc = {a.tpc}, " +
                               $"dpc = {a.dpc}, " +
                               $"crd = {a.crd}, " +
                               $"brd = {a.brd}, " +
                               $"tdc = {a.tdc}, " +
                               $"lid = \"{a.lid}\", " +
                               $"mode = {a.mode}, " +
                               $"ver = {a.ver}, " +
                               $"pp = {a.pp}, " +
                               $"ps = {a.ps}, " +
                               $"pay = pay + {a.pay}, " +
                               $"pay_pc = pay_pc + {a.pay_pc}, " +
                               $"st = {a.st}, " +
                               $"name = \"{b.name}\", " +
                               $"brnk = {b.brnk}, " +
                               $"bcnum = {b.bcnum}, " +
                               $"lcnum = {b.lcnum}, " +
                               $"volt = {b.volt}, " +
                               $"gold = {b.gold}, " +
                               $"lmid = {b.lmid}, " +
                               $"lgrd = {b.lgrd}, " +
                               $"lsrt = {b.lsrt}, " +
                               $"ltab = {b.ltab}, " +
                               $"splv = {b.splv}, " +
                               $"pref = {b.pref}, " +
                               $"lcid = {b.lcid}, " +
                               $"hat = {b.hat}, " +
                               $"custom = \"{custom}\", " +
                               $"last_tips = {last_tips} WHERE refid = \"{a.rid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void bst_2_write_player_item(string refid, RecordBSTItem i) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    "INSERT INTO bst_player2_item VALUES (" +
                    $"\"{refid}\", {i.type}, {i.id}, {i.param}, {i.count}) " +
                    $"ON DUPLICATE KEY UPDATE param = {i.param}, count = {i.count}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void bst_2_write_stage(RecordBSTStage s) {
            //using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
            //    conn.Open();
            //    string query =
            //        $"SELECT * FROM bst_player2_stagedata WHERE user_id = {s.user_id} AND select_music_id = {s.select_music_id} AND select_grade = {s.select_grade}";
            //    MySqlCommand cmd = new MySqlCommand(query, conn);
            //    MySqlDataReader reader = cmd.ExecuteReader();
            //    if (reader.Read()) {
            //        int medal = int.Parse(reader["result_medal"].ToString());
            //        if (int.Parse(s.result_medal) > medal) {
            //            medal = int.Parse(s.result_medal);
            //        }
            //        int score = int.Parse(reader["result_score"].ToString());
            //        using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
            //            conn2.Open();
            //            if (int.Parse(s.result_score) > score) {
            //                string q = "UPDATE bst_player2_stagedata SET " +
            //                           $"location_id = \"{s.location_id}\", " +
            //                           $"result_clear_gauge = {s.result_clear_gauge}, " +
            //                           $"result_score = {s.result_score}, " +
            //                           $"result_max_combo = {s.result_max_combo}, " +
            //                           $"result_grade = {s.result_grade}, " +
            //                           $"result_medal = {medal}, " +
            //                           $"result_fanta = {s.result_fanta}, " +
            //                           $"result_great = {s.result_great}, " +
            //                           $"result_fine = {s.result_fine}, " +
            //                           $"result_miss = {s.result_miss} " +
            //                           $"WHERE user_id = {s.user_id} AND select_music_id = {s.select_music_id} AND select_grade = {s.select_grade}";
            //                MySqlCommand cmd2 = new MySqlCommand(q, conn2);
            //                cmd2.ExecuteNonQuery();
            //            } else {
            //                string q = "UPDATE bst_player2_stagedata SET " +
            //                           $"result_medal = {medal} " +
            //                           $"WHERE user_id = {s.user_id} AND select_music_id = {s.select_music_id} AND select_grade = {s.select_grade}";
            //                MySqlCommand cmd2 = new MySqlCommand(q, conn2);
            //                cmd2.ExecuteNonQuery();
            //            }
            //        }
            //    } else {
                    using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                        conn2.Open();
                        string q =
                            "INSERT INTO bst_player2_stagedata VALUES (" +
                            $"{s.user_id}, " +
                            $"\"{s.location_id}\", " +
                            $"{s.select_music_id}, " +
                            $"{s.select_grade}, " +
                            $"{s.result_clear_gauge}, " +
                            $"{s.result_score}, " +
                            $"{s.result_max_combo}, " +
                            $"{s.result_grade}, " +
                            $"{s.result_medal}, " +
                            $"{s.result_fanta}, " +
                            $"{s.result_great}, " +
                            $"{s.result_fine}, " +
                            $"{s.result_miss}) ";
                        MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                        cmd2.ExecuteNonQuery();
                    }
            //    }
            //}
        }

        public static void bst_2_write_course(RecordBSTCourse c) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =
                    "INSERT INTO bst_player2_course_data VALUES (" +
                    $"{c.user_id}, {c.course_id}, {c.gauge}, {c.score}, {c.grade}, {c.medal}, {c.combo}, {c.fanta}, {c.great}, {c.fine}, {c.miss}, \"{c.lid}\")";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void bst_2_end_player(string refid) {
            using (MySqlConnection conn = new MySqlConnection(ConnStr)) {
                conn.Open();
                string query =$"SELECT * FROM bst_player2_data WHERE refid = \"{refid}\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) {
                    using (MySqlConnection conn2 = new MySqlConnection(ConnStr)) {
                        conn2.Open();
                        string q = $"DELETE FROM card_bind WHERE refid = \"{refid}\" AND model = \"NBT:J:A:A:2016111400\"";
                        MySqlCommand cmd2 = new MySqlCommand(q, conn2);
                        cmd2.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
