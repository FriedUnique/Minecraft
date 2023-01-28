interface ISaveable {
    byte[] GetSaveData();
    void OnUnload();
    void OnLoad(object[] args);
}