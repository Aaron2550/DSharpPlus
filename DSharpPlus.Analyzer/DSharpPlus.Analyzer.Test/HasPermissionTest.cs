﻿namespace DSharpPlus.Analyzer.Test;

public class HasPermissionTest
{
    public void DoesItHaveIt(Permissions permissions)
    {
        if ((permissions & Permissions.Administrator) != 0)
        {
            return;
        }
    }
}
