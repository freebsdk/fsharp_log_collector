module DBWriteTask
open System
open System.Data
open System.Threading
open MySql.Data
open MySql.Data.MySqlClient
open Packet
open IniMgr
open LogMgr

//https://dev.mysql.com/doc/connector-net/en/connector-net-programming.html



type DBWriteTask () =
    
    let mutable conn_ = null


    member x.Init() = 

        let conn_str = sprintf "server=%s;uid=%s;pwd=%s;database=%s" 
                            (IniMgr.Instance.FindStr("db_ip").Value)
                            (IniMgr.Instance.FindStr("db_id").Value)
                            (IniMgr.Instance.FindStr("db_pw").Value)
                            (IniMgr.Instance.FindStr("db_name").Value)
        conn_ <- new MySql.Data.MySqlClient.MySqlConnection(conn_str)
        conn_.Open();



    member x.HandleEvents(packet_q : PacketQueue) =

        try
            let packet = packet_q.Dequeue()

            let tokens = (packet.Message()).Split[|'|'|]
            if tokens.Length >= 2 then 
                let sql = sprintf "INSERT INTO %s VALUES(%s)" tokens.[0] tokens.[1]
                let cmd = new MySqlCommand(sql, conn_)
                let rdr = cmd.ExecuteNonQuery()
                ()
            else
                failwithf "Invalid packet message : (%s)" (packet.Message())
        with
        | _ -> 
            Thread.Sleep(IniMgr.Instance.FindInt("db_write_task_idle_sleep_msec").Value)
