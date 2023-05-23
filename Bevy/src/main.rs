use bevy::{
    diagnostic::{Diagnostics, FrameTimeDiagnosticsPlugin},
    math::vec2,
    prelude::*,
    render::camera::ScalingMode,
    sprite::MaterialMesh2dBundle,
    window::PresentMode,
};
use nanorand::{Rng, WyRand};

const WINDOW_WIDTH: f32 = 800.0;
const WINDOW_HEIGHT: f32 = 600.0;
const ORTHOSIZE: f32 = 5.0;
const XBOUND: f32 = ORTHOSIZE * (WINDOW_WIDTH / WINDOW_HEIGHT) - 0.05;
const YBOUND: f32 = ORTHOSIZE - 0.07;
const GRAVITY: f32 = 0.007;
const MAXVEL: f32 = 0.13;
const BUNNYSCALE: f32 = 0.0135;

#[derive(Component)]
struct Bunny;

#[derive(Component)]
struct Velocity(Vec2);

#[derive(Component)]
struct StatsText;

#[derive(Resource)]
struct Counter(usize);

#[derive(Resource)]
struct Rand(WyRand);

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
        .insert_resource(Counter(0))
        .insert_resource(Rand(WyRand::new()))
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

    commands.spawn(Camera2dBundle {
        projection: OrthographicProjection {
            scaling_mode: ScalingMode::FixedVertical(ORTHOSIZE * 2.),
            ..default()
        },
        ..default()
    });
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
        mesh: meshes.add(shape::Quad::new(vec2(4.25, 1.6)).into()).into(),
        material: materials.add(ColorMaterial::from(Color::BLACK)),
        transform: Transform::from_translation(Vec3::new(-XBOUND, YBOUND, 1.0)),
        ..default()
    });

    // spawn initial bunnies
    for _count in 0..10 {
        commands.spawn((
            Bunny,
            SpriteBundle {
                texture: texture.clone(),
                transform: Transform {
                    scale: Vec3::splat(BUNNYSCALE),
                    translation: Vec3::new(-XBOUND, YBOUND, 0.0),
                    ..default()
                },
                ..default()
            },
            Velocity(Vec2::new(
                rng.0.generate::<f32>() * MAXVEL,
                rng.0.generate::<f32>() * MAXVEL - (MAXVEL / 2.0),
            )),
        ));
    }
    counter.0 = 10;
}

fn move_bunnies(
    mut bunny_query: Query<(&mut Velocity, &mut Transform), With<Bunny>>,
    mut rng: ResMut<Rand>,
) {
    for (mut velocity, mut transform) in &mut bunny_query {
        transform.translation += velocity.0.extend(0.0);
        velocity.0.y -= GRAVITY;

        if transform.translation.x > XBOUND {
            velocity.0.x *= -1.0;
            transform.translation.x = XBOUND;
        } else if transform.translation.x < -XBOUND {
            velocity.0.x *= -1.0;
            transform.translation.x = -XBOUND;
        }

        if transform.translation.y < -YBOUND {
            velocity.0.y *= -0.85;
            transform.translation.y = -YBOUND;
            transform.rotation = Quat::from_rotation_z(rng.0.generate::<f32>() * 0.2 - 0.1);
            if rng.0.generate::<f32>() > 0.5 {
                velocity.0.y += rng.0.generate::<f32>() * 0.1;
            }
        } else if transform.translation.y > YBOUND {
            velocity.0.y = 0.0;
            transform.translation.y = YBOUND;
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
                    commands.spawn((
                        Bunny,
                        SpriteBundle {
                            texture: texture.clone(),
                            transform: Transform {
                                scale: Vec3::splat(BUNNYSCALE),
                                translation: Vec3::new(-XBOUND, YBOUND, 0.0),
                                ..default()
                            },
                            ..default()
                        },
                        Velocity(Vec2::new(
                            rng.0.generate::<f32>() * MAXVEL,
                            rng.0.generate::<f32>() * MAXVEL - (MAXVEL / 2.0),
                        )),
                    ));
                }
                counter.0 += 100;
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
            text.sections[1].value = format!("\nBunnies: {}", counter.0)
        }
    }
}
