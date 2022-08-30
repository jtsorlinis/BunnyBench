function love.load()
  love.window.setTitle("Love2D Bunnymark")
  love.window.setMode(800, 600)
  love.window.setVSync(0)
  BunnySprite = love.graphics.newImage("bunny.png")
  Bunnies = {}
  Count = 0

  Gravity = 0.5
  MinX = 0
  MaxX = 780
  MinY = 0
  MaxY = 565

  for i = 1, 10, 1 do
    local bunny = {
      x = 0,
      y = 0,
      vx = love.math.random() * 10,
      vy = love.math.random() * 10 - 5,
      rot = 0
    }
    table.insert(Bunnies, bunny)
    Count = Count + 1
  end

end

function love.update()
  if 1.0 / love.timer.getDelta() > 59 then
    for i = 1, 100, 1 do
      local bunny = {
        x = 0,
        y = 0,
        vx = love.math.random() * 10,
        vy = love.math.random() * 10 - 5,
        rot = 0
      }
      table.insert(Bunnies, bunny)
      Count = Count + 1
    end
  end
end

function love.draw()

  for k, v in pairs(Bunnies) do
    v.x = v.x + v.vx
    v.y = v.y + v.vy
    v.vy = v.vy + Gravity

    if v.x > MaxX then
      v.vx = v.vx * -1
      v.x = MaxX
    elseif v.x < MinX then
      v.vx = v.vx * -1
      v.x = MinX
    end

    if v.y > MaxY then
      v.vy = v.vy * -0.85
      v.y = MaxY
      v.rot = (love.math.random() - 0.5) * 0.2
      if love.math.random() > 0.5 then
        v.vy = v.vy - love.math.random() * 6
      end
    elseif v.y < MinY then
      v.vy = 0
      v.y = MinY
    end
    love.graphics.draw(BunnySprite, v.x, v.y, v.rot)
  end

  love.graphics.setColor(0, 0, 0)
  love.graphics.rectangle("fill", 0, 0, 110, 50)
  love.graphics.setColor(255, 255, 255)
  love.graphics.print('FPS: ' .. tostring(love.timer.getFPS()), 5, 5)
  love.graphics.print('Bunnies: ' .. tostring(Count), 5, 30)
end
