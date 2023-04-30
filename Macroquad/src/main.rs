use macroquad::prelude::*;

struct Bunny {
    pos: Vec2,
    vel: Vec2,
    rot: f32,
}

#[macroquad::main("BunnyBench")]
async fn main() {
    let min_x: f32 = 0.;
    let max_x = screen_width() - 26.;
    let min_y: f32 = 0.;
    let max_y = screen_height() - 37.;
    let gravity = 0.5;

    let bunny_tex: Texture2D = load_texture("assets/bunny.png").await.unwrap();
    let mut bunnies: Vec<Bunny> = Vec::new();

    for _ in 0..10 {
        bunnies.push(Bunny {
            pos: vec2(min_x, min_y),
            vel: vec2(rand::gen_range(0., 10.), rand::gen_range(-5., 5.)),
            rot: 0.,
        });
    }

    loop {
        clear_background(Color::from_rgba(51, 165, 231, 1));

        for bunny in &mut bunnies {
            bunny.pos += bunny.vel;
            bunny.vel.y += gravity;

            if bunny.pos.x > max_x {
                bunny.vel.x *= -1.;
                bunny.pos.x = max_x;
            } else if bunny.pos.x < min_x {
                bunny.vel.x *= -1.;
                bunny.pos.x = min_x;
            }

            if bunny.pos.y > max_y {
                bunny.vel.y *= -0.85;
                bunny.pos.y = max_y;
                bunny.rot = rand::gen_range(-0.1, 0.1);
                if rand::gen_range(0., 1.) > 0.5 {
                    bunny.vel.y -= rand::gen_range(0., 6.);
                }
            } else if bunny.pos.y < min_y {
                bunny.vel.y = 0.;
                bunny.pos.y = min_y;
            }

            draw_texture_ex(
                bunny_tex,
                bunny.pos.x,
                bunny.pos.y,
                WHITE,
                DrawTextureParams {
                    rotation: bunny.rot,
                    ..Default::default()
                },
            );
        }

        if get_frame_time() < 0.016 {
            for _ in 0..100 {
                bunnies.push(Bunny {
                    pos: vec2(min_x, min_y),
                    vel: vec2(rand::gen_range(0., 10.), rand::gen_range(-5., 5.)),
                    rot: 0.,
                });
            }
        }

        // ui
        draw_rectangle(0., 0., 130.0, 40.0, BLACK);
        draw_text(
            format!("FPS:{}", get_fps()).as_str(),
            5.0,
            15.0,
            20.0,
            WHITE,
        );
        draw_text(
            format!("Bunnies:{}", bunnies.len()).as_str(),
            5.0,
            32.0,
            20.0,
            WHITE,
        );

        next_frame().await
    }
}
