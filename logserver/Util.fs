module Util
open System
open System.Threading




let rec Forever f =
    f()
    Forever f




let Lock (lockobj:obj) f =
    Monitor.Enter lockobj
    try
        f()
    finally
        Monitor.Exit lockobj




let UnixTimeStamp() =
    (int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
