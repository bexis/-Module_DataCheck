<script>
	
	//let container = document.getElementById("plotids");
	//let plotids = container.getAttribute("plots");

	let files;
	let plotsid = [];

	let textareaPlots = "Insert plots commy seperated.";

	const handleSubmit = () => {
		let reader = readFile(csvFile);
		let plots= csvToArray(reader, ",");
	}

	function readFile(file)
	{
		const reader = new FileReader();
		const input = file;

		reader.onload = function (e) {
        const text = e.target.result;
		const data = csvToArray(text);
		textareaPlots = JSON.stringify(data);

        document.write(JSON.stringify(data));

      	};

		return reader.readAsText(input);
	}

	function csvToArray(str, delimiter = ",") 
	{
		let plots = str.slice(str.indexOf("\n") + 1).split("\n");
		let array = rows.map(function (row) {
    	let values = row.split(delimiter);
    	let el = headers.reduce(function (object, header, index) {
      		object[header] = values[index];
      		return object;
    	}, {});
    		return el;
		});

		return array;
	}

</script>


<main>
	Upload file or enter plot ids to the textfield<br><br>
	<form on:submit|preventDefault={handleSubmit}>

		<div class="box"><input type="file" bind:files><br>
		<input type="submit" value="Submit" /></div>
	
	</form>

	<div class="box"><textarea value={textareaPlots}></textarea></div>

	

{#if files && files[0]}
	<p>
		{files[0].name}
	</p>
{/if}
</main>

<style>
	main {
		text-align: left;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}
	textarea { 
		width: 100px; height: 200px; 
	}

	.box {
     float: left;
     padding: 20px;
     background: #eee;
}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>