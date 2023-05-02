using System;

[Serializable]
public class GameDate
{
    public int month;
    public int week;
    public int day;
    public int countDay;
    private const float DAYOFMONTH = 28f;

    public void Init(int countDay)
    {
        this.countDay = countDay;

        Calculate();
    }

    public void AddDay()
    {
        countDay++;
        Calculate();
    }

    private void Calculate()
    {
        // var countDay = LevelManager.Instance.Level.countDay;
        month = (int)Math.Truncate(countDay / DAYOFMONTH);
        double dayOfMonth = month * DAYOFMONTH;
        week = (int)Math.Truncate((countDay - dayOfMonth) / 7);
        double dayOfMonthAndWeek = (month * DAYOFMONTH) + (week * 7);
        day = (int)(countDay - dayOfMonthAndWeek);
    }
}