namespace Sfs2X.WebSocketSharp.Net
{
	internal enum InputChunkState
	{
		None,
		Data,
		DataEnded,
		Trailer,
		End
	}
}
