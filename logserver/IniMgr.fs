module IniMgr
open System.IO
open System.Xml
open System.Collections.Generic







type IniMgr() =

    let dic_ = new Dictionary< string, string >()
    static member val Instance = IniMgr()




    member x.Load(file_name : string) =
        
        let xml_doc = XmlDocument()
        try
            if File.Exists(file_name) = false then
                failwithf "file_not_found : %s" file_name
            else
                xml_doc.Load(file_name)
                xml_doc.SelectNodes "config/info"
                |> Seq.cast<XmlNode>
                |> Seq.iter(fun node -> dic_.Add( node.Attributes.["key"].Value, node.Attributes.["val"].Value) )
        with
        | ex -> printfn "[Exception] %s" ex.Message
        ()




    member x.FindStr(key : string) =
        match dic_.TryGetValue(key) with
        | true, value -> Some(value)
        | _ -> None



    member x.FindInt(key : string) =
        match dic_.TryGetValue(key) with
        | true, value -> Some( value |> int )
        | _ -> None
