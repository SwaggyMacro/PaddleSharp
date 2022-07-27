﻿using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online
{
    public record OnlineClassificationModel(string name, Uri uri, ModelVersion version)
    {
        public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, name);

        public async Task<FileClassificationModel> DownloadAsync(CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(RootDirectory);
            string paramsFile = Path.Combine(RootDirectory, "inference.pdiparams");

            if (!File.Exists(paramsFile))
            {
                string localTarFile = Path.Combine(RootDirectory, uri.Segments.Last());
                if (!File.Exists(localTarFile))
                {
                    Console.WriteLine($"Downloading {name} model from {uri}");
                    await Utils.DownloadFile(uri, localTarFile, cancellationToken);
                }

                Console.WriteLine($"Extracting {localTarFile} to {RootDirectory}");
                using (IArchive archive = ArchiveFactory.Open(localTarFile))
                {
                    archive.WriteToDirectory(RootDirectory);
                }

                File.Delete(localTarFile);
            }

            return new FileClassificationModel(RootDirectory);
        }

        /// <summary>
        /// Slim quantized model for text angle classification
        /// (Size: 2.1M)
        /// </summary>
        public readonly static OnlineClassificationModel ChineseMobileSlimV2 = new("ch_ppocr_mobile_slim_v2.0_cls", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_slim_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Original model for text angle classification
        /// (Size: 1.38M)
        /// </summary>
        public readonly static OnlineClassificationModel ChineseMobileV2 = new("ch_ppocr_mobile_v2.0_cls", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"), ModelVersion.V2);
    }
}
