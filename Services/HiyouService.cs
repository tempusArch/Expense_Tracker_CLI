using tracker.Models;
using tracker.Interfaces;
using System.Text.Json;

namespace tracker.Services {
    public class HiyouServices : IHiyouServices {
        private static string fileName = "hiyouData.json";
        private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        public int addHiyou(double d, string m) {
            try {
                var risuto = new List<Hiyou>();
                var newOne = new Hiyou {
                    ID = getHiyouID(),
                    Date = DateTime.Now,
                    Description = m,
                    Amount = d
                };

                bool dekitaka = createFileIfNotExist();

                if (dekitaka) {
                    risuto = getRisutoFromJson();
                    risuto?.Add(newOne);
                    updateJsonFile(risuto?? []);
                    return newOne.ID;
                }

                return 0;

            } catch (Exception e) {
                Console.WriteLine("Failed at adding new Hiyou. Error - " + e.Message);
                throw;
            }
        }

        public bool updateDescription(int i, string m) {
            if (!File.Exists(filePath))
                return false;

            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var oldOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (oldOne != null) {
                    var newOne = new Hiyou {
                        ID = i,
                        Date = oldOne.Date,
                        Description = m,
                        Amount = oldOne.Amount
                    };
                    risuto.Remove(oldOne);
                    risuto.Add(newOne);
                    updateJsonFile(risuto);
                    return true;
                }
            }

            return false;
        }

        public bool updateAmount(int i, double d) {
            if (!File.Exists(filePath))
                return false;

            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var oldOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (oldOne != null) {
                    var newOne = new Hiyou {
                        ID = i,
                        Date = oldOne.Date,
                        Description = oldOne.Description,
                        Amount = d
                    };
                    risuto.Remove(oldOne);
                    risuto.Add(newOne);
                    updateJsonFile(risuto);
                    return true;
                }
            }

            return false;
        }

        public bool deleteHiyou(int i) {
            if (!File.Exists(filePath))
                return false;

            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var deletingOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (deletingOne != null) {
                    risuto.Remove(deletingOne);
                    updateJsonFile(risuto);
                    return true;
                }
            }

            return false;
        }

        public List<Hiyou> getAllHiyouRisuto() {
            try {
                return getRisutoFromJson();

            } catch (Exception e) {
                Console.WriteLine("Failed at fetching all hiyous. Error - " + e.Message);
                throw;
            }
        }

        public double countTotal() {
            try {
                if (!File.Exists(filePath))
                    return 0;
               
                var risuto = getRisutoFromJson();
                return risuto.Sum(n => n.Amount);

            } catch (Exception e) {
                Console.WriteLine("Failed at counting all hiyous. Error - " + e.Message);
                throw;
            }
        }

        public double countTotalSpecificMonth(int i) {
            try {
                if (!File.Exists(filePath))
                    return 0;

                var risuto = getRisutoFromJson();
                var filteredRisuto = risuto.Where(n => n.Date.Month == i && n.Date.Year == DateTime.Now.Year).ToList();
                return filteredRisuto.Sum(n => n.Amount);

            } catch (Exception e) {
                Console.WriteLine("Failed at counting specific month's hiyou. Error - " + e.Message);
                throw;
            } 
        }

        public List<string> getAllCommands() {
            return new List<string> {
                @"add \Description\ \Amount\ - To add a new hiyou, type add with hiyou description and amount",
                @"updatedes \ID\ \Description\ - To update the discription of a hiyou, type update with hiyou ID and description",
                @"updateamo \ID\ \Amount\ - To update the amount of a hiyou, type update with hiyou ID and amount",
                @"delete \ID\ - To delete a hiyou, type delete with hiyou ID",
                "list - To list all hiyous",
                "count - To count amount of all hiyous",
                @"specific \Month(number)\ - To count specific month's hiyou of current year",
                "exit - To exit from app",
                "clear - To clear console window"
            };
        }

        #region  helper methods
        private int getHiyouID() {
            if (!File.Exists(filePath))
                return 1;
            else {
                string currentJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(currentJson)) {
                    var risuto = JsonSerializer.Deserialize<List<Hiyou>>(currentJson);
                    if (risuto != null && risuto.Count > 0)
                        return risuto.OrderBy(n => n.ID).Last().ID + 1;
                }
            }

            return 1;
        }

        private bool createFileIfNotExist() {
            try {
                if (!File.Exists(filePath))
                    using (FileStream i = File.Create(filePath))
                        Console.WriteLine($"Succeeded in creating {fileName}.");

                return true;

            } catch (Exception e) {
                Console.WriteLine($"Failed at creating {fileName}. Error - " + e.Message);
                throw;
            }
        }

        private List<Hiyou> getRisutoFromJson() {
            if (!File.Exists(filePath))
                return new List<Hiyou>();

            string currentJson = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(currentJson))
                return JsonSerializer.Deserialize<List<Hiyou>>(currentJson)?? [];

            return new List<Hiyou>();
        }

        private void updateJsonFile(List<Hiyou> i) {
            string updatedJson = JsonSerializer.Serialize<List<Hiyou>>(i);
            File.WriteAllText(filePath, updatedJson);
        }
        #endregion
    }
}