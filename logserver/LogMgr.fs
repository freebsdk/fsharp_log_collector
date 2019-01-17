module LogMgr
open System
open System.IO
open IniMgr





type LogMgr() =
    
    static member val Instance = LogMgr()

    member x.WriteLine(msg : string) =
        let file_name = sprintf "%s/%s.log" 
                            (IniMgr.Instance.FindStr("log_dir").Value) 
                            (DateTime.Now.ToString("yyyyMMdd"))
        let final_msg = sprintf "[%s] %s" (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) msg
        use sw = new StreamWriter(file_name, true)
        sw.WriteLine(final_msg)
        