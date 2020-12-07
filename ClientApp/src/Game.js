import React, { Component } from 'react'

class Game extends Component {
    constructor(props) {
        super(props);

        this.state = { game: null }
    }
	
	componentDidMount() {
		this.populateGame();
	}
	
	showGameList() {
		this.props.showGameListHandler();
	}
	
	render() {
        return (
            <div>
                <button onClick={this.showGameList.bind(this)}>Leave</button>
            </div>
        )
    }
	
	async populateGame() {
		const response = await fetch('games/' + this.props.gameCode);
		const game = await response.json();
		this.setState((state) => {
            return { game: game };
        });
	}
}

export default Game;