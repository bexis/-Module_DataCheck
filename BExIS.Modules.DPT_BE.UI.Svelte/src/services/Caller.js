import {Api} from "@bexis2/svelte-bexis2-core-ui";

export const countPlots = async (plots) => {
    //console.log("plots", plots);
    try {
      const response = await Api.post('/DPT_BE/PlotProfiling/CountPlots', {plots});
      return response.data;
    } catch (error) {
      console.error(error);
    }
};