import turtle

def apply_rules(cmd):
    if cmd == 'F':
        return 'FF'
    elif cmd == 'X':
        return 'F-[[X]+X]+F[+FX]-X'

    return cmd


def process_string(old_string):
    new_string = ''

    for ch in old_string:
        new_string += apply_rules(ch)

    return new_string


def create_l_system(iterations, axiom):
    result = axiom

    for _ in range(iterations):
        result = process_string(result)

    return result

def draw_l_system(l_system_string, t, distance, angle):
    stack = []

    for cmd in l_system_string:
        if cmd == 'F':
            t.forward(distance)
        elif cmd == '+':
            t.left(angle)
        elif cmd == '-':
            t.right(angle)
        elif cmd == '[':
            stack.append((t.position(), t.heading()))
        elif cmd == ']':
            position, rotation = stack.pop()
            t.penup()
            t.goto(position)
            t.setheading(rotation)
            t.pendown()


def main():

    # Initialize turtle
    t = turtle.Turtle()
    wn = turtle.Screen()
    # wn.tracer(0)
    t.speed(0)
    t.left(90)
    t.penup()
    t.goto(0, -wn.window_height() // 2 + 20)
    t.pendown()

    # L-systems parameters
    iterations = 5
    angle = 22.5
    distance = 5
    axiom = 'X'
    l_system_string = create_l_system(iterations, axiom)

    draw_l_system(l_system_string, t, distance, angle)

    # wn.update()
    # Wait for user to close window
    wn.mainloop()

if __name__ == "__main__":
    main()