use bevy::{
    diagnostic::{Diagnostics, FrameTimeDiagnosticsPlugin},
    math::vec2,
    prelude::*,
    sprite::MaterialMesh2dBundle,
    window::PresentMode,
};
use rand::{thread_rng, Rng};

const WINDOW_WIDTH: f32 = 800.0;
const WINDOW_HEIGHT: f32 = 600.0;
const MINX: f32 = -(WINDOW_WIDTH / 2.0) + 10.0;
const MAXX: f32 = -MINX;
const MINY: f32 = (WINDOW_HEIGHT / 2.0) - 16.0;
const MAXY: f32 = -MINY;
const GRAVITY: f32 = 0.5;

#[derive(Component)]
struct Bunny {
    velocity: Vec3,
}

#[derive(Component)]
struct StatsText;

#[derive(Component)]
struct Counter {
    count: usize,
}

fn main() {
    App::new()
        .insert_resource(WindowDescriptor {
            width: WINDOW_WIDTH,
            height: WINDOW_HEIGHT,
            present_mode: PresentMode::AutoNoVsync,
            ..default()
        })
        .add_plugins(DefaultPlugins)
        .add_plugin(FrameTimeDiagnosticsPlugin::default())
        .insert_resource(Counter { count: 0 })
        .add_startup_system(setup)
        .add_system(move_bunnies)
        .add_system(display_stats)
        .add_system(spawn_bunnies)
        .run();
}

#[derive(Deref)]
struct BunnyTexture(Handle<Image>);

fn setup(
    mut commands: Commands,
    asset_server: Res<AssetServer>,
    mut counter: ResMut<Counter>,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<ColorMaterial>>,
) {
    let texture = asset_server.load("bunny.png");
    let mut rng = thread_rng();

    commands.spawn_bundle(Camera2dBundle::default());
    commands.insert_resource(BunnyTexture(texture.clone()));

    commands
        .spawn_bundle(
            TextBundle::from_sections([
                TextSection::new(
                    "FPS:",
                    TextStyle {
                        font: asset_server.load("PTRoot.ttf"),
                        font_size: 20.0,
                        color: Color::WHITE,
                    },
                ),
                TextSection::new(
                    "\nBunnies: 0",
                    TextStyle {
                        font: asset_server.load("PTRoot.ttf"),
                        font_size: 20.0,
                        color: Color::WHITE,
                    },
                ),
            ])
            .with_style(Style {
                position_type: PositionType::Absolute,
                position: UiRect {
                    top: Val::Px(5.0),
                    left: Val::Px(5.0),
                    ..default()
                },
                ..default()
            }),
        )
        .insert(StatsText);

    commands.spawn_bundle(MaterialMesh2dBundle {
        mesh: meshes
            .add(shape::Quad::new(vec2(250.0, 70.0)).into())
            .into(),
        material: materials.add(ColorMaterial::from(Color::BLACK)),
        transform: Transform::from_translation(Vec3::new(MINX, MINY, 1.0)),
        ..default()
    });

    // spawn initial bunnies
    for _count in 0..10 {
        commands
            .spawn_bundle(SpriteBundle {
                texture: texture.clone(),
                transform: Transform {
                    translation: Vec3::new(MINX, MINY, 0.0),
                    ..default()
                },
                ..default()
            })
            .insert(Bunny {
                velocity: Vec3::new(rng.gen_range(0.0..10.0), rng.gen_range(-5.0..5.0), 0.0),
            });
    }
    counter.count = 10;
}

fn move_bunnies(mut bunny_query: Query<(&mut Bunny, &mut Transform)>) {
    let mut rng = thread_rng();

    for (mut bunny, mut transform) in &mut bunny_query {
        transform.translation.x += bunny.velocity.x;
        transform.translation.y += bunny.velocity.y;
        bunny.velocity.y -= GRAVITY;

        if transform.translation.x > MAXX {
            bunny.velocity.x *= -1.0;
            transform.translation.x = MAXX;
        } else if transform.translation.x < MINX {
            bunny.velocity.x *= -1.0;
            transform.translation.x = MINX;
        }

        if transform.translation.y < MAXY {
            bunny.velocity.y *= -0.85;
            transform.translation.y = MAXY;
            transform.rotation = Quat::from_rotation_z(rng.gen_range(-0.1..0.1));
            if rng.gen_range(0.0..1.0) > 0.5 {
                bunny.velocity.y += rng.gen_range(0.0..6.0);
            }
        } else if transform.translation.y > MINY {
            bunny.velocity.y = 0.0;
            transform.translation.y = MINY;
        }
    }
}

fn spawn_bunnies(
    mut commands: Commands,
    texture: Res<BunnyTexture>,
    diagnostics: Res<Diagnostics>,
    mut counter: ResMut<Counter>,
) {
    let mut rng = thread_rng();

    if let Some(fps) = diagnostics.get(FrameTimeDiagnosticsPlugin::FPS) {
        if let Some(average) = fps.average() {
            if average > 59.0 {
                for _count in 0..100 {
                    commands
                        .spawn_bundle(SpriteBundle {
                            texture: texture.clone(),
                            transform: Transform {
                                translation: Vec3::new(MINX, MINY, 0.0),
                                ..default()
                            },
                            ..default()
                        })
                        .insert(Bunny {
                            velocity: Vec3::new(
                                rng.gen_range(0.0..10.0),
                                rng.gen_range(-5.0..5.0),
                                0.0,
                            ),
                        });
                }
                counter.count += 100;
            }
        }
    }
}

fn display_stats(
    diagnostics: Res<Diagnostics>,
    counter: ResMut<Counter>,
    mut query: Query<&mut Text, With<StatsText>>,
) {
    let mut text = query.single_mut();
    if let Some(fps) = diagnostics.get(FrameTimeDiagnosticsPlugin::FPS) {
        if let Some(average) = fps.average() {
            text.sections[0].value = format!("FPS: {average:.1}");
            text.sections[1].value = format!("\nBunnies: {}", counter.count)
        }
    }
}
