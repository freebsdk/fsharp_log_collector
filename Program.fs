open System
open System.Threading
open Util
open Packet
open NetworkTask
open DBWriteTask
open IniMgr




let ActivateNetTask(net_task : NetworkTask, packet_q) = async {
    net_task.Open("", IniMgr.Instance.FindInt("service_port").Value)
    Forever <| fun() ->
        net_task.HandleEvents(packet_q)
    net_task.Close()
    }




let ActivateDBWriteTask(db_write_task : DBWriteTask, packet_q) = async {
    db_write_task.Init()
    Forever <| fun() ->
        db_write_task.HandleEvents(packet_q)
    }




[<EntryPoint>]
let main argv = 
    
    if argv.Length < 1 then
        printfn "Please specify config.xml"
    else
        let net_task = NetworkTask()
        let db_write_task = DBWriteTask()
        let packet_q = PacketQueue()

        IniMgr.Instance.Load(argv.[0])
        Async.Start( ActivateDBWriteTask(db_write_task, packet_q) )
        Async.Start( ActivateNetTask(net_task, packet_q) )
        Forever <| fun() ->
            Thread.Sleep(1000)

    0 // return an integer exit code
