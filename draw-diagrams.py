import matplotlib.pyplot as plt
import networkx as nx

def draw_monolithic_architecture():
    """
    Draws the monolithic architecture diagram for the Conway's Game of Life API.
    """
    G = nx.DiGraph()

    # Nodes
    G.add_node("Client", pos=(0, 3))
    G.add_node("API", pos=(0, 2))
    G.add_node("Middleware", pos=(0, 1))
    G.add_node("Service Layer", pos=(0, 0))
    G.add_node("Redis", pos=(-1, -1))
    G.add_node("Database", pos=(1, -1))

    # Edges
    G.add_edges_from([
        ("Client", "API"),
        ("API", "Middleware"),
        ("Middleware", "Service Layer"),
        ("Service Layer", "Redis"),
        ("Service Layer", "Database")
    ])

    # Draw the graph
    pos = nx.get_node_attributes(G, 'pos')
    nx.draw(G, pos, with_labels=True, node_size=3000, node_color='lightblue', font_size=10, font_weight='bold', arrowsize=20)
    plt.title("Monolithic Architecture")
    plt.show()

def draw_microservices_architecture():
    """
    Draws the microservices architecture diagram for the Conway's Game of Life API.
    """
    G = nx.DiGraph()

    # Nodes
    G.add_node("Client", pos=(0, 4))
    G.add_node("API Gateway", pos=(0, 3))
    G.add_node("Authentication Service", pos=(-2, 2))
    G.add_node("Board Management Service", pos=(0, 2))
    G.add_node("Game Logic Service", pos=(2, 2))
    G.add_node("Redis", pos=(0, 1))
    G.add_node("Database", pos=(-1, 0))
    G.add_node("Message Broker", pos=(1, 0))

    # Edges
    G.add_edges_from([
        ("Client", "API Gateway"),
        ("API Gateway", "Authentication Service"),
        ("API Gateway", "Board Management Service"),
        ("API Gateway", "Game Logic Service"),
        ("Authentication Service", "API Gateway"),
        ("Board Management Service", "Redis"),
        ("Board Management Service", "Database"),
        ("Game Logic Service", "Redis"),
        ("Game Logic Service", "Message Broker"),
        ("Board Management Service", "Message Broker")
    ])

    # Draw the graph
    pos = nx.get_node_attributes(G, 'pos')
    nx.draw(G, pos, with_labels=True, node_size=3000, node_color='lightgreen', font_size=10, font_weight='bold', arrowsize=20)
    plt.title("Microservices Architecture")
    plt.show()

# Draw both architectures
draw_monolithic_architecture()
draw_microservices_architecture()
