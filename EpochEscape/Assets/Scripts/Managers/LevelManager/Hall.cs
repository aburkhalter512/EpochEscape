using UnityEngine;

public class Hall : Room
{
    private static int s_count = 0;

    public Hall()
    {
        s_count++;
    }

    public void Dispose()
    {
        s_count--;

        if (s_count < 0)
            s_count = 0;

        base.Dispose();
    }

    public override string GetRoomType()
    {
        return "Hall";
    }

    public override int GetRoomCount()
    {
        return s_count;
    }
}
