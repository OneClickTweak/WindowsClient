﻿using OneClickTweak.Settings;

namespace OneClickTweak.Handlers;

public class SqliteHandler : ISettingsHandler
{
    public string Name => "Sqlite";

    public bool IsVersionMatch(Setting item)
    {
        throw new NotImplementedException();
    }
}