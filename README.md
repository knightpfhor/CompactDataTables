CompactDataTables
=================

This is a JSON.Net data converter for data tables that produces more compact JSON.

The regular JSON.Net DataTable converter produces lovely looking JSON that similar to this:

[
  {
    "My String Field":"My string value",
    "My Int Field":42,
    "My Boolean Field":true
  },
  {
    "My String Field":"Another string value",
    "My Int Field":96,
    "My Boolean Field":false
  }
]

This makes it easy to understand the data that is being sent and to consume the data in clients that don't know anything about the DataTable class.

It does have a couple of drawbacks though.  If you're trying to deserialze this back into a DataTable instance it has to try and deduce the data type from the data which works in most circumstances.  However this doesn't work if you're trying to transfer binary data (I know, I know, storing binary data in a DB is a bad idea, but not all data designs are perfect) JSON will serialize this as a Base64 encoded string.  When you try to deserialize this there is no meaningful way to distinguish this from a regular string.  You also have to make a decision about whether you want to deserialize numbers with decimal points as a floating point  number or as a decimal.

This is where the CompactDataTable converter comes it.  It includes meta-data about the column types so the client can deserialize it back to the way it was on the other end.  It also doesn't repeat field names for every entry, so if you're returning more than two rows of data, the JSON should be smaller (hence the Compact).  When put through the CompactDataTable converter the JSON would look like this:
[
  {
    "My String Field":0,
    "My Int Field":4,
    "My Boolean Field":1
  },
  [
    "My string value",
    42,
    true
  ],
  [
    "Another string value",
    96,
    false
  ]
]
