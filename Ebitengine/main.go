package main

import (
	"fmt"
	"image/color"
	_ "image/png"
	"log"
	"math/rand"

	"github.com/hajimehoshi/ebiten/v2"
	"github.com/hajimehoshi/ebiten/v2/ebitenutil"
)

type Bunny struct {
	x   float32
	y   float32
	vx  float32
	vy  float32
	rot float32
}

var minX float32 = 0
var maxX float32 = 775
var minY float32 = 0
var maxY float32 = 565

var gravity float32 = 0.5

var bunnyTex *ebiten.Image

var bunnies []*Bunny

var op *ebiten.DrawImageOptions = &ebiten.DrawImageOptions{}

func init() {
	var err error
	bunnyTex, _, err = ebitenutil.NewImageFromFile("assets/bunny.png")
	for i := 0; i < 10; i++ {
		bunnies = append(bunnies, &Bunny{
			x:  minX,
			y:  minY,
			vx: rand.Float32() * 10,
			vy: rand.Float32()*10 - 5,
		})
	}

	if err != nil {
		log.Fatal(err)
	}
}

type Game struct{}

func (g *Game) Update() error {
	// add while over 59fps
	if ebiten.CurrentFPS() > 59 {
		for i := 0; i < 100; i++ {
			bunnies = append(bunnies, &Bunny{
				x:  minX,
				y:  minY,
				vx: rand.Float32() * 10,
				vy: rand.Float32()*10 - 5,
			})
		}
	}

	// move bunnies
	for _, bunny := range bunnies {
		bunny.x += bunny.vx
		bunny.y += bunny.vy
		bunny.vy += gravity

		if bunny.x > maxX {
			bunny.vx *= -1
			bunny.x = maxX
		} else if bunny.x < minX {
			bunny.vx *= -1
			bunny.x = minX
		}

		if bunny.y > maxY {
			bunny.vy *= -0.85
			bunny.y = maxY
			bunny.rot = (rand.Float32() - 0.5) * 0.2
			if rand.Float32() > 0.5 {
				bunny.vy -= rand.Float32() * 6
			}
		} else if bunny.y < minY {
			bunny.vy = 0
			bunny.y = minY
		}
	}
	return nil
}

func (g *Game) Draw(screen *ebiten.Image) {

	for _, bunny := range bunnies {
		op.GeoM.Rotate(float64(bunny.rot))
		op.GeoM.Translate(float64(bunny.x), float64(bunny.y))
		screen.DrawImage(bunnyTex, op)
		op.GeoM.Reset()
	}
	ebitenutil.DrawRect(screen, 0, 0, 100, 45, color.Black)
	ebitenutil.DebugPrintAt(screen, fmt.Sprintf("FPS: %f", ebiten.CurrentFPS()), 5, 5)
	ebitenutil.DebugPrintAt(screen, fmt.Sprintf("Bunnies: %d", len(bunnies)), 5, 25)
}

func (g *Game) Layout(outsideWidth, outsideHeight int) (screenWidth, screenHeight int) {
	return 800, 600
}

func main() {
	ebiten.SetWindowSize(800, 600)
	ebiten.SetWindowTitle("Hello, World!")
	ebiten.SetMaxTPS(-1)
	if err := ebiten.RunGame(&Game{}); err != nil {
		log.Fatal(err)
	}
}
