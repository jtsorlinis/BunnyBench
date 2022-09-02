package com.gdx.bunnymark;

import java.util.ArrayList;
import java.util.Random;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer.ShapeType;
import com.badlogic.gdx.utils.ScreenUtils;

public class GDXBunnymark extends ApplicationAdapter {
	SpriteBatch batch;
	ShapeRenderer sr;
	Texture img;
	Random random = new Random();
	BitmapFont font;
	ArrayList<Bunny> bunnies = new ArrayList<>();

	float gravity = 0.5f;

	int minX = 0;
	int maxX = 775;
	int minY = 565;
	int maxY = 0;

	@Override
	public void create() {
		font = new BitmapFont();
		batch = new SpriteBatch();
		sr = new ShapeRenderer();
		img = new Texture("bunny.png");

		for (int i = 0; i < 10; i++) {
			Bunny bunny = new Bunny();
			bunny.pos.x = minX;
			bunny.pos.y = minY;
			bunny.vel.x = random.nextFloat() * 10;
			bunny.vel.y = random.nextFloat() * 10 - 5;
			bunnies.add(bunny);
		}

	}

	@Override
	public void render() {
		float fps = 1 / Gdx.graphics.getDeltaTime();

		if (fps > 59) {
			for (int i = 0; i < 100; i++) {
				Bunny bunny = new Bunny();
				bunny.pos.x = minX;
				bunny.pos.y = minY;
				bunny.vel.x = random.nextFloat() * 10;
				bunny.vel.y = random.nextFloat() * 10 - 5;
				bunnies.add(bunny);
			}
		}

		ScreenUtils.clear(0, 0, 0, 1);
		batch.begin();

		for (Bunny bunny : bunnies) {
			bunny.pos.x += bunny.vel.x;
			bunny.pos.y += bunny.vel.y;
			bunny.vel.y -= gravity;

			if (bunny.pos.x > maxX) {
				bunny.vel.x *= -1;
				bunny.pos.x = maxX;
			} else if (bunny.pos.x < minX) {
				bunny.vel.x *= -1;
				bunny.pos.x = minX;
			}

			if (bunny.pos.y < maxY) {
				bunny.vel.y *= -0.85;
				bunny.pos.y = maxY;
				bunny.rot = random.nextFloat() * 12 - 6;
				if (random.nextFloat() > 0.5) {
					bunny.vel.y += random.nextFloat() * 6;
				}
			} else if (bunny.pos.y > minY) {
				bunny.vel.y = 0;
				bunny.pos.y = minY;
			}
			batch.draw(img, bunny.pos.x, bunny.pos.y, .5f, .5f, 26, 37, 1, 1, bunny.rot, 0, 0, 26, 37, false, false);
		}

		batch.end();
		sr.begin(ShapeType.Filled);
		sr.setColor(Color.BLACK);
		sr.rect(0, 555, 0, 0, 130, 100, 1, 1, 0);
		sr.end();
		batch.begin();
		font.draw(batch, "FPS: " + Math.round(fps), 5, 595);
		font.draw(batch, "Bunnies: " + bunnies.size(), 5, 575);
		batch.end();
	}

	@Override
	public void dispose() {
		batch.dispose();
		img.dispose();
	}
}
