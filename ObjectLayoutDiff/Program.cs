using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SonicRetro.SonLVL.API;
using S1ObjectEntry = SonicRetro.SonLVL.API.S1.S1ObjectEntry;
using S2ObjectEntry = SonicRetro.SonLVL.API.S2.S2ObjectEntry;
using S2NAObjectEntry = SonicRetro.SonLVL.API.S2NA.S2NAObjectEntry;
using S3KObjectEntry = SonicRetro.SonLVL.API.S3K.S3KObjectEntry;
using SCDObjectEntry = SonicRetro.SonLVL.API.SCD.SCDObjectEntry;

namespace ObjectLayoutDiff
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length > 4)
			{
				LevelData.littleendian = false;
				EngineVersion format = (EngineVersion)Enum.Parse(typeof(EngineVersion), args[1]);
				switch (format)
				{
					case EngineVersion.S1:
					case EngineVersion.SCD:
					case EngineVersion.S2:
					case EngineVersion.S2NA:
					case EngineVersion.S3K:
						break;
					case EngineVersion.SCDPC:
					case EngineVersion.SKC:
						LevelData.littleendian = true;
						break;
					default:
						ShowHelp();
						return;
				}
				switch (args[0].ToLowerInvariant())
				{
					case "create":
						List<ObjectEntry> origobjs = LoadObjects(File.ReadAllBytes(args[2]), format);
						List<ObjectEntry> newobjs = LoadObjects(File.ReadAllBytes(args[3]), format);
						for (int i = 0; i < origobjs.Count; i++)
							for (int j = 0; j < newobjs.Count; j++)
								if (origobjs[i].GetBytes().FastArrayEqual(newobjs[j].GetBytes()))
								{
									origobjs.RemoveAt(i--);
									newobjs.RemoveAt(j);
									break;
								}
						DiffData diff = new DiffData();
						diff.format = format;
						if (newobjs.Count > 0)
							diff.added = newobjs.Select(a => a.Data).ToList();
						else
							diff.added = null;
						if (origobjs.Count > 0)
							diff.removed = origobjs.Select(a => a.Data).ToList();
						else
							diff.removed = null;
						IniSerializer.Serialize(diff, args[4]);
						break;
					case "apply":
						diff = IniSerializer.Deserialize<DiffData>(args[2]);
						if (diff.format != format)
						{
							Console.WriteLine("Error: Diff file does not match selected format.");
							return;
						}
						List<ObjectEntry> objs = LoadObjects(File.ReadAllBytes(args[3]), format);
						Type objtype = null;
						switch (format)
						{
							case EngineVersion.S1:
								objtype = typeof(S1ObjectEntry);
								break;
							case EngineVersion.S2:
								objtype = typeof(S2ObjectEntry);
								break;
							case EngineVersion.S2NA:
								objtype = typeof(S2NAObjectEntry);
								break;
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								objtype = typeof(S3KObjectEntry);
								break;
							case EngineVersion.SCD:
							case EngineVersion.SCDPC:
								objtype = typeof(SCDObjectEntry);
								break;
						}
						if (diff.removed != null)
						{
							if (diff.removed.Count > 0)
								for (int i = 0; i < objs.Count; i++)
									for (int j = 0; j < diff.removed.Count; j++)
										if (objs[i].Data == diff.removed[j])
										{
											objs.RemoveAt(i--);
											diff.removed.RemoveAt(j);
											break;
										}
							foreach (string s in diff.removed)
								Console.WriteLine("Could not find object \"{0}\" for deletion.", s);
						}
						if (diff.added != null)
							foreach (string s in diff.added)
							{
								ObjectEntry obj = (ObjectEntry)Activator.CreateInstance(objtype);
								obj.Data = s;
								objs.Add(obj);
							}
						objs.Sort();
						List<byte> tmp = new List<byte>();
						foreach (ObjectEntry obj in objs)
							tmp.AddRange(obj.GetBytes());
						switch (format)
						{
							case EngineVersion.S1:
								tmp.AddRange(new byte[] { 0xFF, 0xFF });
								while (tmp.Count % S1ObjectEntry.Size > 0)
									tmp.Add(0);
								break;
							case EngineVersion.S2:
								tmp.AddRange(new byte[] { 0xFF, 0xFF });
								while (tmp.Count % S2ObjectEntry.Size > 0)
									tmp.Add(0);
								break;
							case EngineVersion.S2NA:
								tmp.AddRange(new byte[] { 0xFF, 0xFF });
								while (tmp.Count % S2NAObjectEntry.Size > 0)
									tmp.Add(0);
								break;
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								tmp.AddRange(new byte[] { 0xFF, 0xFF });
								while (tmp.Count % S3KObjectEntry.Size > 0)
									tmp.Add(0);
								break;
							case EngineVersion.SCD:
							case EngineVersion.SCDPC:
								tmp.Add(0xFF);
								while (tmp.Count % SCDObjectEntry.Size > 0)
									tmp.Add(0xFF);
								break;
						}
						File.WriteAllBytes(args[4], tmp.ToArray());
						break;
					default:
						ShowHelp();
						return;
				}
			}
			else
				ShowHelp();
		}

		static List<ObjectEntry> LoadObjects(byte[] file, EngineVersion format)
		{
			List<ObjectEntry> result = new List<ObjectEntry>();
			switch (format)
			{
				case EngineVersion.S1:
					for (int oa = 0; oa < file.Length; oa += S1ObjectEntry.Size)
					{
						if (ByteConverter.ToUInt16(file, oa) == 0xFFFF) break;
						result.Add(new S1ObjectEntry(file, oa));
					}
					break;
				case EngineVersion.S2:
					for (int oa = 0; oa < file.Length; oa += S2ObjectEntry.Size)
					{
						if (ByteConverter.ToUInt16(file, oa) == 0xFFFF) break;
						result.Add(new S2ObjectEntry(file, oa));
					}
					break;
				case EngineVersion.S2NA:
					for (int oa = 0; oa < file.Length; oa += S2NAObjectEntry.Size)
					{
						if (ByteConverter.ToUInt16(file, oa) == 0xFFFF) break;
						result.Add(new S2NAObjectEntry(file, oa));
					}
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					for (int oa = 0; oa < file.Length; oa += S3KObjectEntry.Size)
					{
						if (ByteConverter.ToUInt16(file, oa) == 0xFFFF) break;
						result.Add(new S3KObjectEntry(file, oa));
					}
					break;
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					for (int oa = 0; oa < file.Length; oa += SCDObjectEntry.Size)
					{
						if (ByteConverter.ToUInt64(file, oa) == 0xFFFFFFFFFFFFFFFF) break;
						result.Add(new SCDObjectEntry(file, oa));
					}
					break;
			}
			return result;
		}

		static void ShowHelp()
		{
			Console.WriteLine("Usage: ObjectLayoutDiff CREATE format file1 file2 difffile");
			Console.WriteLine();
			Console.WriteLine("Compares the object layout files file1 and file2 and creates a diff file.");
			Console.WriteLine();
			Console.WriteLine("Usage: ObjectLayoutDiff APPLY format difffile infile outfile");
			Console.WriteLine();
			Console.WriteLine("Applies the information in the diff file to the object layout file infile and saves the result as outfile.");
			Console.WriteLine();
		}
	}

	public class DiffData
	{
		public EngineVersion format;
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> added;
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> removed;
	}
}
