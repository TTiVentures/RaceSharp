﻿namespace RaceSharp.Application.WorkFlowCore
{
    public class Resource
    {
        //public Bucket Bucket { get; set; }

        public string Name { get; set; }

        //public int Version { get; set; }

        public string ContentType { get; set; }

        public string Content { get; set; }

        public byte[] CompiledContent { get; set; }

    }

    public enum Bucket { Lambda, Step, Protobuf, File };
}
