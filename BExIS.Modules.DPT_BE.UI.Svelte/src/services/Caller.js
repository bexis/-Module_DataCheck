import {Api} from "@bexis2/svelte-bexis2-core-ui";

export const countPlots = async (plots) => {

  let data = {
    plots: plots
    };

    try {
      console.log(data);
      const response = await Api.post('/api/DPT_BE/CountPlots', data);
      return response.data;
    } catch (error) {
      console.error(error);
    }
};