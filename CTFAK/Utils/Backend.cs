using System.ComponentModel;
using CTFAK.GUI;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using static CTFAK.MMFParser.EXE.Exe;

namespace CTFAK.Utils
{
    public static class Backend
    {
        public static void DumpSounds(MainForm form,bool load,bool save)
        {
            using (var worker = new BackgroundWorker())
            {
                if (Instance.GameData.GameChunks.GetChunk<SoundBank>() == null) return;
                form.SetSoundElements(true);
                worker.DoWork += (senderA, eA) => { Instance.GameData.GameChunks.GetChunk<SoundBank>().Read(save); };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    form.SetSoundElements(false);
                    MainForm.IsDumpingSounds = false;
                };
                worker.RunWorkerAsync();
            }
        }
        public static void DumpImages(MainForm form,bool load,bool save)
        {
            using (var worker = new BackgroundWorker())
            {
                if (Instance.GameData.GameChunks.GetChunk<ImageBank>() == null) return;
                form.SetImageElements(true);
                worker.DoWork += (senderA, eA) =>
                {
                    Instance.GameData.GameChunks.GetChunk<ImageBank>().PreloadOnly = false;
                    Instance.GameData.GameChunks.GetChunk<ImageBank>().Read(load,save);
                };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    form.SetImageElements(false);
                    MainForm.IsDumpingImages = false;
                };
                worker.RunWorkerAsync();
            }
        }
        public static void DumpMusics(MainForm form,bool load,bool save)
        {
            using (var worker = new BackgroundWorker())
            {
                if (Instance.GameData.GameChunks.GetChunk<MusicBank>() == null) return;
                form.SetMusicElements(true);
                worker.DoWork += (senderA, eA) => { Instance.GameData.GameChunks.GetChunk<MusicBank>().Read(save); };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    form.SetMusicElements(false);
                    MainForm.IsDumpingMusics = false;
                };
                worker.RunWorkerAsync();
            }
        }
        
        
    }
}