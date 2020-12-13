import React, { Component } from 'react'

class CreateGame extends Component {
	constructor(props) {
		super(props);

		this.state = {
			answerSeconds: 0,
			challengeSeconds: 0
		};

		this.updateAnswerSeconds = this.updateAnswerSeconds.bind(this);
		this.updateChallengeSeconds = this.updateChallengeSeconds.bind(this);
		this.createGame = this.createGame.bind(this);
    }

	updateAnswerSeconds(event) {
		this.setState({
			answerSeconds: event.target.value
		});
    }

	updateChallengeSeconds(event) {
		this.setState({
			challengeSeconds: event.target.value
		});
	}

	createGame() {
		this.props.showGameHandler();

        this.props.connection.invoke("CreateGame", this.props.name, this.state.answerSeconds, this.state.challengeSeconds).catch(function (err) {
            return console.error(err.toString());
        });
    }

    render() {
		return (
			<div className="right align-right create-game">
				<span>Answer time (seconds): <input className="number-field" type="text" value={this.state.answerSeconds} onChange={this.updateAnswerSeconds} /></span><br />
				<span>Challenge time (seconds): <input className="number-field" type="text" value={this.state.challengeSeconds} onChange={this.updateChallengeSeconds} /></span><br />
				<br />
				<button onClick={this.createGame}>Create game</button>
			</div>
		);
    }
}

export default CreateGame;