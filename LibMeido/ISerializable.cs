namespace LibMeido;

internal interface ISerializable {
	//string FileHeader { get; set; }
	void Serialize(BinaryWriter binaryWriter);
}
