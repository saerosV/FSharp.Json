open FSharp.Json
open NUnit.Framework

type Order = {
    [<JsonField("id")>] Id: string
    [<JsonField("qty")>] Qty: int
    [<JsonField("price")>] Price: float
}

type OrderQty = {
    [<JsonField("id")>] Id: string
    [<JsonField("new_qty")>] NewQty: int
}

[<JsonUnion(Mode = UnionMode.CaseKeyDiscriminatorField, CaseKeyField="event")>]
type OrderEvent1 =
| [<JsonUnionCase("created")>] Created of Order
| [<JsonUnionCase("amended")>] Amended of OrderQty

[<Test>]
let ``Union with discriminator field serialize/deserialize`` () =
    let expected = OrderEvent1.Amended { Id = "id123"; NewQty = 123 }
    let jsonExpected = """{"event":"amended","id":"id123","new_qty":123}"""
    let jsonActual = Json.serializeU expected
    Assert.AreEqual(jsonExpected, jsonActual)
    let actual = Json.deserialize<OrderEvent1> jsonExpected
    Assert.AreEqual(expected, actual)

[<JsonUnion(Mode = UnionMode.CaseKeyAsFieldName)>]
type OrderEvent2 =
| [<JsonUnionCase("created")>] Created of Order
| [<JsonUnionCase("amended")>] Amended of OrderQty

[<Test>]
let ``Union as wrapper object serialize/deserialize`` () =
    let expected = OrderEvent2.Amended { Id = "id123"; NewQty = 123 }
    let jsonExpected = """{"amended":{"id":"id123","new_qty":123}}"""
    let jsonActual = Json.serializeU expected
    Assert.AreEqual(jsonExpected, jsonActual)
    let actual = Json.deserialize<OrderEvent2> jsonExpected
    Assert.AreEqual(expected, actual)