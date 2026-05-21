namespace mbt.webapi.BuiltIn;

public static class SpaRoutes
{
    public static string IncrementalFundsViewItem(string id) => $"/incremental-funds/view/{id}";

    public static string TransfersEditItem(string transferId)
    {
        return $"/transfers/edit/{transferId}";
    }

    public static string TransfersViewItem(string transferId)
    {
        return $"/transfers/view/{transferId}";
    }
}
