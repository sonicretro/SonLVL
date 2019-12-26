using SonicRetro.SonLVL.API;

namespace S3SSEdit
{
	class ProjectFile
	{
		[IniName("S3Stage")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public string[] S3Stages { get; set; }
		public string SKStageSet { get; set; }
		public string BlueSphereChunkSet { get; set; }

		public static ProjectFile Load(string filename)
		{
			return IniSerializer.Deserialize<ProjectFile>(filename);
		}
	}
}
