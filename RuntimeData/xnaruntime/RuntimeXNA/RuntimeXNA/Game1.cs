using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if !WINDOWS_PHONE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;
using RuntimeXNA.Application;
//01
#if WINDOWS_PHONE
using Microsoft.Advertising.Mobile.Xna;
#endif
//01END 

namespace RuntimeXNA
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatchEffect spriteBatch;
//02
#if WINDOWS_PHONE
        public static AdGameComponent adGameComponent;
#endif
//02END
        public bool bPreviousActive;
        public bool bInitialActivation;

        CRunApp application;
#if !WINDOWS_PHONE
        public GamerServicesComponent gamerServices;
#endif
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
//03
#if WINDOWS_PHONE
            AdGameComponent.Initialize(this, "test_client");
            adGameComponent = AdGameComponent.Current;
            Components.Add(adGameComponent);
#endif
//03END
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated +=
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated +=
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
#endif
            bPreviousActive = false;
            bInitialActivation = true;
        }
#if WINDOWS_PHONE
        void GameActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            if (application != null && application.run != null)
            {
                if (application.XNAObject != null)
                {
                    application.run.callEventExtension(application.XNAObject, 3, 0);
                }
                application.run.resume();
            }
        }

        void GameDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            if (application != null && application.run != null)
            {
                if (application.XNAObject != null)
                {
                    application.run.callEventExtension(application.XNAObject, 2, 0);
                }
                application.run.pause();
            }
        }
#endif

        protected override void Initialize()
        {
#if !WINDOWS_PHONE
            GamerServicesDispatcher.WindowHandle = Window.Handle;
            GamerServicesDispatcher.Initialize(Services);
#endif
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatchEffect(Content, GraphicsDevice);
            IsMouseVisible = true;
            BinaryRead.Data cca = Content.Load<BinaryRead.Data>("Application");
            CFile cfile = new CFile(cca.data);
            application = new CRunApp(this, cfile);
            if (application.load())
            {
                application.startApplication();
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Activation / Deactivation of the application
            if (bPreviousActive != IsActive)
            {
                bPreviousActive=IsActive;
                if (bInitialActivation == false)
                {
                    if (application.run != null)
                    {
                        if (IsActive)
                            application.run.resume();
                        else
                            application.run.pause();
                    }
                }
                else
                {
                    bInitialActivation = false;
                }
            }

            double time = gameTime.TotalGameTime.TotalMilliseconds;
            if (application.playApplication(false, time) == false)
            {
                this.Exit();
            }
#if !WINDOWS_PHONE
            if (GamerServicesDispatcher.IsInitialized)
            {
                GamerServicesDispatcher.Update();
            }
#endif
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            double time = gameTime.TotalGameTime.TotalMilliseconds;

            application.draw();

            base.Draw(gameTime);
        }
    }
}
