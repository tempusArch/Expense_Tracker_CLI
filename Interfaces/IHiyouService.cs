using tracker.Models;

namespace tracker.Interfaces {
    public interface IHiyouServices {
        int addHiyou(double d, string m);
        bool updateDescription(int i, string m);
        bool updateAmount(int i, double d);
        bool deleteHiyou(int i);
        List<Hiyou> getAllHiyouRisuto();
        double countTotal();
        double countTotalSpecificMonth(int i);
        List<string> getAllCommands();
    }
}
