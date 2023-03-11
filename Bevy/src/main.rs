use bevy::{
    diagnostic::{Diagnostics, FrameTimeDiagnosticsPlugin},
    math::vec2,
    prelude::*,
    sprite::MaterialMesh2dBundle,
    window::PresentMode,
};
use nanorand::{Rng, WyRand};

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

#[derive(Resource)]
struct Counter {
    count: usize,
}

#[derive(Resource)]
struct Rand {
    rng: WyRand,
}

fn main() {
    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                resolution: (WINDOW_WIDTH, WINDOW_HEIGHT).into(),
                present_mode: PresentMode::AutoNoVsync,
                ..default()
            }),
            ..default()
        }))
        .add_plugin(FrameTimeDiagnosticsPlugin::default())
        .insert_resource(Counter { count: 0 })
        .insert_resource(Rand { rng: WyRand::new() })
        .add_startup_system(setup)
        .add_system(move_bunnies)
        .add_system(display_stats)
        .add_system(spawn_bunnies)
        .run();
}

#[derive(Resource, Deref)]
struct BunnyTexture(Handle<Image>);

fn setup(
    mut commands: Commands,
    asset_server: Res<AssetServer>,
    mut counter: ResMut<Counter>,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<ColorMaterial>>,
    mut rng: ResMut<Rand>,
) {
    let texture = asset_server.load("bunny.png");

    commands.spawn(Camera2dBundle::default());
    commands.insert_resource(BunnyTexture(texture.clone()));

    commands
        .spawn(
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

    commands.spawn(MaterialMesh2dBundle {
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
            .spawn(SpriteBundle {
                texture: texture.clone(),
                transform: Transform {
                    translation: Vec3::new(MINX, MINY, 0.0),
                    ..default()
                },
                ..default()
            })
            .insert(Bunny {
                velocity: Vec3::new(
                    rng.rng.generate::<f32>() * 10.0,
                    rng.rng.generate::<f32>() * 10.0 - 5.0,
                    0.0,
                ),
            });
    }
    counter.count = 10;
}

fn move_bunnies(mut bunny_query: Query<(&mut Bunny, &mut Transform)>, mut rng: ResMut<Rand>) {
    for (mut bunny, mut transform) in &mut bunny_query {
        transform.translation += bunny.velocity;
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
            transform.rotation = Quat::from_rotation_z(rng.rng.generate::<f32>() * 0.2 - 0.1);
            if rng.rng.generate::<f32>() > 0.5 {
                bunny.velocity.y += rng.rng.generate::<f32>() * 6.0;
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
    mut rng: ResMut<Rand>,
) {
    if let Some(fps) = diagnostics.get(FrameTimeDiagnosticsPlugin::FPS) {
        if let Some(average) = fps.average() {
            if average > 59.0 {
                for _count in 0..100 {
                    commands
                        .spawn(SpriteBundle {
                            texture: texture.clone(),
                            transform: Transform {
                                translation: Vec3::new(MINX, MINY, 0.0),
                                ..default()
                            },
                            ..default()
                        })
                        .insert(Bunny {
                            velocity: Vec3::new(
                                rng.rng.generate::<f32>() * 10.0,
                                rng.rng.generate::<f32>() * 10.0 - 5.0,
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
