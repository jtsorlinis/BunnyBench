package com.gdx.bunnymark;

import com.badlogic.gdx.backends.lwjgl3.Lwjgl3Application;
import com.badlogic.gdx.backends.lwjgl3.Lwjgl3ApplicationConfiguration;

// Please note that on macOS your application needs to be started with the -XstartOnFirstThread JVM argument
public class DesktopLauncher {
	public static void main(String[] arg) {
		org.lwjgl.system.Configuration.GLFW_LIBRARY_NAME.set("glfw_async");
		Lwjgl3ApplicationConfiguration config = new Lwjgl3ApplicationConfiguration();
		config.setWindowedMode(800, 600);
		config.setForegroundFPS(0);
		config.useVsync(false);
		config.setTitle("GDXBunnymark");
		new Lwjgl3Application(new GDXBunnymark(), config);
	}
}
